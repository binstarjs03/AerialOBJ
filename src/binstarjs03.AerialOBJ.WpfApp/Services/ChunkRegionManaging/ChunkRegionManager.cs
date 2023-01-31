using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using binstarjs03.AerialOBJ.Core;
using binstarjs03.AerialOBJ.Core.MinecraftWorld;
using binstarjs03.AerialOBJ.Core.Primitives;
using binstarjs03.AerialOBJ.WpfApp.Factories;
using binstarjs03.AerialOBJ.WpfApp.Models;
using binstarjs03.AerialOBJ.WpfApp.Primitives;
using binstarjs03.AerialOBJ.WpfApp.Services.ChunkRegionManaging;
using binstarjs03.AerialOBJ.WpfApp.Services.ChunkRendering;
using binstarjs03.AerialOBJ.WpfApp.Services.Dispatcher;
using binstarjs03.AerialOBJ.WpfApp.Services.IOService;
using binstarjs03.AerialOBJ.WpfApp.Settings;

using CommunityToolkit.Mvvm.ComponentModel;

namespace binstarjs03.AerialOBJ.WpfApp.Services;

[ObservableObject]
public partial class ChunkRegionManager : IChunkRegionManager
{
    private const int s_initialRegionBufferSize = 15;
    private const int s_initialChunkBufferSize = s_initialRegionBufferSize * IRegion.TotalChunkCount;
    private readonly Random _random = new();

    // Dependencies -----------------------------------------------------------
    private readonly Setting _setting;
    private readonly IRegionDiskLoader _regionProvider;
    private readonly RegionDataImageModelFactory _regionDataImageModelFactory;
    private readonly IChunkRenderer _chunkRenderer;
    private readonly IDispatcher _dispatcher;
    private readonly IChunkLoadingPattern _chunkPattern;

    // Threadings -------------------------------------------------------------
    private CancellationTokenSource _stoppingCts = new(); // use this when pausing
    private CancellationTokenSource _reinitializingCts = new(); // use this when reinitializing
    private readonly ManualResetEventSlim _redrawEvent = new(false);

    private readonly ReferenceWrap<bool> _isPendingRegionTaskRunning = new() { Value = false };
    private Task _pendingRegionTask = Task.CompletedTask;

    private readonly ReferenceWrap<bool> _isRedrawTaskRunning = new() { Value = false };
    private Task _redrawTask = Task.CompletedTask;

    // leave one cpu thread for both main thread and pending region loader
    private readonly Dictionary<uint, Task> _pendingChunkTasks = new(Environment.ProcessorCount);
    private readonly ReferenceWrap<uint> _newChunkLoaderTaskId = new() { Value = default };

    // Manager States ---------------------------------------------------------
    private readonly List<PointZ<int>> _errorRegions = new();
    private readonly HashSet<PointZ<int>> _errorChunks = new();
    private int _heightLevel;
    private readonly ReaderWriterLockSlim _heightLevelLock = new();

    private PointZRange<int> _visibleRegionRange = default;
    private PointZRange<int> _visibleChunkRange = default;
    private readonly ReaderWriterLockSlim _visibleRegionRangeLock = new();
    private readonly ReaderWriterLockSlim _visibleChunkRangeLock = new();

    private readonly Dictionary<PointZ<int>, RegionDataImageModel> _loadedRegions = new(s_initialRegionBufferSize);
    private readonly Dictionary<PointZ<int>, RegionDataImageModel> _cachedRegions = new(s_initialRegionBufferSize);
    private readonly List<PointZ<int>> _pendingRegions = new(s_initialRegionBufferSize);
    private readonly ReferenceWrap<PointZ<int>?> _workedRegion = new() { Value = null };

    private readonly Dictionary<PointZ<int>, ChunkModel> _renderedChunks = new(s_initialChunkBufferSize);
    private readonly HashSet<PointZ<int>> _pendingChunksSet = new(s_initialChunkBufferSize);
    private readonly List<PointZ<int>> _pendingChunks = new(s_initialChunkBufferSize);
    private readonly object _pendingChunkLock = new();
    private readonly List<PointZ<int>> _workedChunks = new(Environment.ProcessorCount);

    public ChunkRegionManager(
        Setting setting,
        IRegionDiskLoader regionProvider,
        RegionDataImageModelFactory regionImageModelFactory,
        IChunkRenderer chunkRenderer,
        IDispatcher dispatcher,
        IChunkLoadingPattern chunkPattern)
    {
        _setting = setting;
        _regionProvider = regionProvider;
        _regionDataImageModelFactory = regionImageModelFactory;
        _chunkRenderer = chunkRenderer;
        _dispatcher = dispatcher;
        _chunkPattern = chunkPattern;
    }

    public PointZRange<int> VisibleRegionRange => _visibleRegionRange;
    public int VisibleRegionsCount => VisibleRegionRange.Sum;
    public int LoadedRegionsCount => _loadedRegions.Count;
    public int CachedRegionsCount => _cachedRegions.Count;
    public int PendingRegionsCount => _pendingRegions.Count;
    public PointZ<int>? WorkedRegion => _workedRegion.Value;

    public PointZRange<int> VisibleChunkRange => _visibleChunkRange;
    public int VisibleChunksCount => VisibleChunkRange.Sum;
    public int LoadedChunksCount => _renderedChunks.Count;
    public int PendingChunksCount => _pendingChunks.Count;
    public int WorkedChunksCount => _workedChunks.Count;

    private int PendingChunkTasksLimit => _setting.PerformanceSetting.ViewportChunkThreads;

    public event Action<RegionDataImageModel>? RegionLoaded;
    public event Action<RegionDataImageModel>? RegionUnloaded;
    public event ChunkRegionReadingErrorHandler? RegionLoadingException;
    public event ChunkRegionReadingErrorHandler? ChunkLoadingException;

    public void Update(PointZ<float> cameraPos, float unitMultiplier, Size<int> screenSize)
    {
        if (IsVisibleChunkRangeChanged(cameraPos, unitMultiplier, screenSize))
        {
            if (IsVisibleRegionRangeChanged())
                ManageRegions();
            ManageChunks();
        }
    }

    public void UpdateHeightLevel(int heightLevel)
    {
        _heightLevelLock.EnterWriteLock();
        try
        {
            if (_heightLevel != heightLevel)
                _heightLevel = heightLevel;
            UpdateChunkHighestBlockResponsive();
        }
        finally { _heightLevelLock.ExitWriteLock(); }
    }

    private bool IsVisibleChunkRangeChanged(PointZ<float> cameraPos, float unitMultiplier, Size<int> screenSize)
    {
        _visibleChunkRangeLock.EnterWriteLock();
        try
        {
            PointZRange<int> oldVisibleChunkRange = _visibleChunkRange;
            PointZRange<int> newVisibleChunkRange = CalculateVisibleChunkRange(cameraPos, unitMultiplier, screenSize);
            if (newVisibleChunkRange == oldVisibleChunkRange)
                return false;
            _visibleChunkRange = newVisibleChunkRange;
            OnPropertyChanged(nameof(VisibleChunkRange));
            OnPropertyChanged(nameof(VisibleChunksCount));
            return true;
        }
        finally { _visibleChunkRangeLock.ExitWriteLock(); }
    }

    private bool IsVisibleRegionRangeChanged()
    {
        _visibleRegionRangeLock.EnterWriteLock();
        try
        {
            PointZRange<int> oldVisibleRegionRange = _visibleRegionRange;
            PointZRange<int> newVisibleRegionRange = CalculateVisibleRegionRange(_visibleChunkRange);
            if (newVisibleRegionRange == oldVisibleRegionRange)
                return false;
            _visibleRegionRange = newVisibleRegionRange;
            OnPropertyChanged(nameof(VisibleRegionRange));
            OnPropertyChanged(nameof(VisibleRegionsCount));
            return true;
        }
        finally { _visibleRegionRangeLock.ExitWriteLock(); }
    }

    private static PointZRange<int> CalculateVisibleChunkRange(PointZ<float> cameraPos, float unitMultiplier, Size<int> screenSize)
    {
        float pixelPerChunk = unitMultiplier * IChunk.BlockCount; // one unit (or pixel) equal to one block
        Rangeof<int> visibleChunkXRange = calculateSingleAxis(screenSize.Width, pixelPerChunk, cameraPos.X);
        Rangeof<int> visibleChunkZRange = calculateSingleAxis(screenSize.Height, pixelPerChunk, cameraPos.Z);
        return new PointZRange<int>(visibleChunkXRange, visibleChunkZRange);

        static Rangeof<int> calculateSingleAxis(int screenSize, float pixelPerChunk, float cameraPos)
        {
            // Camera chunk in here means which chunk the camera is pointing to.
            float cameraChunk = cameraPos / IChunk.BlockCount;

            // worldScreenCenter represent which world coordinate
            // the center of viewport screen is pointing to
            float worldScreenCenter = screenSize / 2f;

            // min-maxScreenChunk means which chunk that is visible at the edgemost of the viewport
            // that is measured by where the center of viewport is pointing to which world coordinate
            // without taking camera position into account
            float maxScreenChunk = worldScreenCenter / pixelPerChunk;
            float minScreenChunk = -maxScreenChunk;

            int minChunkRange = MathUtils.Floor(cameraChunk + minScreenChunk);
            int maxChunkRange = MathUtils.Floor(cameraChunk + maxScreenChunk);

            return new Rangeof<int>(minChunkRange, maxChunkRange);
        }
    }

    private static PointZRange<int> CalculateVisibleRegionRange(PointZRange<int> visibleChunkRange)
    {
        Rangeof<int> visibleRegionXRange = calculateSingleAxis(visibleChunkRange.XRange);
        Rangeof<int> visibleRegionZRange = calculateSingleAxis(visibleChunkRange.ZRange);
        return new PointZRange<int>(visibleRegionXRange, visibleRegionZRange);

        static Rangeof<int> calculateSingleAxis(Rangeof<int> visibleChunkRange)
        {
            // Calculating region range is easier since we only need to
            // divide the range by how many chunks in region (in single axis)
            int regionMinX = MathUtils.DivFloor(visibleChunkRange.Min, IRegion.ChunkCount);
            int regionMaxX = MathUtils.DivFloor(visibleChunkRange.Max, IRegion.ChunkCount);
            return new Rangeof<int>(regionMinX, regionMaxX);
        }
    }

    private void ManageRegions()
    {
        UnloadCulledRegions();
        QueuePendingRegions();
        RunPendingRegionTask();
        OnPropertyChanged(nameof(LoadedRegionsCount));
        OnPropertyChanged(nameof(PendingRegionsCount));
    }

    private void UnloadCulledRegions()
    {
        // perform boundary range checking for regions outside display frame
        // unload loaded and cancel pending region if outside
        lock (_loadedRegions)
            foreach ((PointZ<int> regionCoords, RegionDataImageModel regionModel) in _loadedRegions)
                if (_visibleRegionRange.IsOutside(regionCoords))
                    UnloadRegion(regionModel);
        // remove all pending region list that is no longer visible
        lock (_pendingRegions)
            _pendingRegions.RemoveAll(_visibleRegionRange.IsOutside);
    }

    private void QueuePendingRegions()
    {

        // this section of code may looks slow but mostly
        // only few regions are visible to the screen at a time
        for (int x = _visibleRegionRange.XRange.Min; x <= _visibleRegionRange.XRange.Max; x++)
            for (int z = _visibleRegionRange.ZRange.Min; z <= _visibleRegionRange.ZRange.Max; z++)
            {
                // skip if already loaded
                PointZ<int> regionCoords = new(x, z);
                lock (_loadedRegions)
                    if (_loadedRegions.ContainsKey(regionCoords))
                        continue;

                // load cached region instantly without having to go through pending queue
                lock (_cachedRegions)
                    if (_cachedRegions.TryGetValue(regionCoords, out RegionDataImageModel? regionDataImageModel))
                    {
                        LoadRegion(regionDataImageModel, true);
                        continue;
                    }

                // fail to fetch from cache, proceed to add it to pending queue,
                // though skip adding to pending queue if it is already there
                lock (_pendingRegions)
                    lock (_workedRegion)
                        if (_loadedRegions.ContainsKey(regionCoords)
                            || _pendingRegions.Contains(regionCoords)
                            || _workedRegion.Value == regionCoords)
                            continue;
                        else
                            _pendingRegions.Add(regionCoords);
            }
    }

    private void RunPendingRegionTask()
    {
        lock (_pendingRegions)
            if (_pendingRegions.Count == 0)
                return;
        lock (_isPendingRegionTaskRunning)
        {
            if (_isPendingRegionTaskRunning.Value)
                return;
            _isPendingRegionTaskRunning.Value = true;
            _pendingRegionTask = Task.Factory.StartNew(ProcessPendingRegion,
                                                       CancellationToken.None,
                                                       TaskCreationOptions.LongRunning,
                                                       TaskScheduler.Default);
        }
    }

    private void ProcessPendingRegion()
    {
        try
        {
            while (!_stoppingCts.IsCancellationRequested || !_reinitializingCts.IsCancellationRequested)
            {
                // wrap whole iteration inside try/finally clause,
                // ensure worked region will always be cleaned no matter what
                try
                {
                    // get random pending region then assign it to worked region
                    if (!tryGetRandomPendingRegion(out PointZ<int> regionCoords))
                        return;

                    // get region and load it if succeed
                    if (!tryGetRegion(regionCoords, out RegionDataImageModel? region))
                        continue;

                    LoadRegion(region);
                    addRegionToCache(region);
                }
                finally { cleanupWorkedRegion(); }
            }
        }
        finally
        {
            lock (_isPendingRegionTaskRunning)
                _isPendingRegionTaskRunning.Value = false;
        }

        bool tryGetRandomPendingRegion(out PointZ<int> regionCoords)
        {
            regionCoords = default;
            // get pending region and set it to worked region atomically, this is
            // to prevent the main thread from corrupting pending region state
            // (e.g after its no longer in pending queue, the main thread adds it
            // back during queue invocation because it does not exist in both
            // pending and worked)
            lock (_pendingRegions)
            {
                if (_pendingRegions.Count == 0)
                    return false;

                int randomIndex = _random.Next(0, _pendingRegions.Count);
                regionCoords = _pendingRegions[randomIndex];

                lock (_workedRegion)
                    _workedRegion.Value = regionCoords;
                _pendingRegions.RemoveAt(randomIndex);
            }
            OnPropertyChanged(nameof(PendingRegionsCount));
            OnPropertyChanged(nameof(WorkedRegion));
            return true;
        }

        bool tryGetRegion(PointZ<int> regionCoords, [NotNullWhen(true)] out RegionDataImageModel? regionModel)
        {
            regionModel = null;
            try
            {
                if (!_regionProvider.TryGetRegion(regionCoords, _reinitializingCts.Token, out IRegion? region))
                    return false;
                regionModel = _regionDataImageModelFactory.Create(region, _reinitializingCts.Token);
                return true;
            }
            catch (TaskCanceledException) { throw; } // quickly terminate if we are reinitializing
            catch (Exception e) { handleException(regionCoords, e); }
            return false;
        }

        void handleException(PointZ<int> regionCoords, Exception e)
        {
            if (_errorRegions.Contains(regionCoords))
                return;
            _dispatcher.InvokeAsync(() => RegionLoadingException?.Invoke(regionCoords, e),
                                    DispatcherPriority.Background,
                                    _reinitializingCts.Token);
            _errorRegions.Add(regionCoords);
        }

        void addRegionToCache(RegionDataImageModel regionModel)
        {
            lock (_cachedRegions)
                _cachedRegions.Add(regionModel.Data.Coords, regionModel);
            OnPropertyChanged(nameof(CachedRegionsCount));

        }

        void cleanupWorkedRegion()
        {
            lock (_workedRegion)
                _workedRegion.Value = null;
            OnPropertyChanged(nameof(WorkedRegion));
        }
    }

    private void LoadRegion(RegionDataImageModel region, bool isMainThread = false)
    {
        // Always load region from main thread to synchronize both crm (this class) and viewport vm state
        if (isMainThread)
            loadRegion();
        else
            _dispatcher.Invoke(loadRegion, DispatcherPriority.Background, _reinitializingCts.Token);
        OnPropertyChanged(nameof(LoadedRegionsCount));

        void loadRegion()
        {
            // both main and pending region threads are synchronized, no need to lock
            // also don't check if region is already loaded, let the exception reveal itself, 
            // because if so, that is definitely a bug in queue method we have to fix
            try
            {
                if (_visibleRegionRange.IsOutside(region.Data.Coords))
                    return;
                _loadedRegions.Add(region.Data.Coords, region);
            }
            catch (ArgumentException) { throw; } // thrown from adding to loadedRegions
            RegionLoaded?.Invoke(region);
        }
    }

    private void UnloadRegion(RegionDataImageModel regionModel)
    {
        // locking is already done before this method called
        _loadedRegions.Remove(regionModel.Data.Coords);
        RegionUnloaded?.Invoke(regionModel);
    }

    private void ManageChunks()
    {
        CancelPendingChunks();
        QueuePendingChunks();
        RunPendingChunkTasks();
        OnPropertyChanged(nameof(LoadedChunksCount));
        OnPropertyChanged(nameof(PendingChunksCount));
    }

    private void CancelPendingChunks()
    {
        lock (_pendingChunkLock)
        {
            _pendingChunks.RemoveAll(_visibleChunkRange.IsOutside);
            _pendingChunksSet.RemoveWhere(_visibleChunkRange.IsOutside);
        }
    }

    private void QueuePendingChunks()
    {
        // this section of code may have lots of overheads as hundreds even tens of hundreds chunks
        // may visible to the screen at a time, plus there are quadruple of locks combined
        for (int x = _visibleChunkRange.XRange.Min; x <= _visibleChunkRange.XRange.Max; x++)
            for (int z = _visibleChunkRange.ZRange.Min; z <= _visibleChunkRange.ZRange.Max; z++)
            {
                PointZ<int> chunkCoords = new(x, z);
                lock (_renderedChunks)
                    lock (_pendingChunkLock)
                        lock (_workedChunks)
                            if (_renderedChunks.ContainsKey(chunkCoords)
                                || _pendingChunksSet.Contains(chunkCoords)
                                || _workedChunks.Contains(chunkCoords))
                                continue;
                            else
                            {
                                _pendingChunks.Add(chunkCoords);
                                _pendingChunksSet.Add(chunkCoords);
                            }
            }
        lock (_pendingChunkLock)
            _pendingChunks.Sort();
    }

    // TODO we may be able to create new class that manages and track pool of tasks
    private void RunPendingChunkTasks()
    {
        lock (_pendingChunkTasks)
        {
            while (_pendingChunkTasks.Count < PendingChunkTasksLimit)
            {
                // reset overflow to zero
                if (_newChunkLoaderTaskId.Value == uint.MaxValue)
                    _newChunkLoaderTaskId.Value = 0;
                uint taskId = _newChunkLoaderTaskId.Value++;
                Task task = new(() => ProcessPendingChunk(taskId), _stoppingCts.Token, TaskCreationOptions.LongRunning);
                _pendingChunkTasks.Add(taskId, task);
                task.Start();
            }
        }
    }

    private void ProcessPendingChunk(uint taskId)
    {
        try
        {
            while (!_stoppingCts.IsCancellationRequested || !_reinitializingCts.IsCancellationRequested)
            {
                PointZ<int>? chunkCoords = null;
                try
                {
                    if (!tryGetRandomPendingChunk(out PointZ<int> chunkCoordsOut))
                        return;
                    chunkCoords = chunkCoordsOut;

                    // get both chunk and region, then load it if succeed
                    if (!tryGetChunkAndRegion(chunkCoords.Value, out IChunk? chunk, out RegionDataImageModel? regionModel))
                        continue;
                    LoadChunk(chunk, regionModel);
                }
                finally { cleanupWorkedChunk(chunkCoords); }
            }
        }
        finally
        {
            lock (_pendingChunkTasks)
                _pendingChunkTasks.Remove(taskId);
        }

        bool tryGetRandomPendingChunk(out PointZ<int> chunkCoords)
        {
            chunkCoords = default;
            lock (_pendingChunkLock)
            {
                if (_pendingChunks.Count == 0)
                    return false;
                int index = _chunkPattern.GetPendingChunkIndex(_pendingChunks.Count);
                chunkCoords = _pendingChunks[index];
                lock (_workedChunks)
                    _workedChunks.Add(chunkCoords);
                _pendingChunks.RemoveAt(index);
                _pendingChunksSet.Remove(chunkCoords);
            }
            OnPropertyChanged(nameof(PendingChunksCount));
            OnPropertyChanged(nameof(WorkedChunksCount));
            return true;
        }

        bool tryGetChunkAndRegion(PointZ<int> chunkCoords, [NotNullWhen(true)] out IChunk? chunk, [NotNullWhen(true)] out RegionDataImageModel? regionModel)
        {
            // get the underlying region
            chunk = null;
            regionModel = GetRegionModelForChunk(chunkCoords, out RegionStatus status);
            if (regionModel is null)
            {
                // can't get region because it isn't loaded yet, put chunk back to pending list and
                // "lets hope" in next encounter, region is loaded. We may refactor this to make it more optimal
                if (status == RegionStatus.Pending)
                    lock (_pendingChunkLock)
                    {
                        _pendingChunks.Add(chunkCoords);
                        _pendingChunksSet.Add(chunkCoords);
                    }
                OnPropertyChanged(nameof(PendingChunksCount));
                return false;
            }
            try
            {
                PointZ<int> chunkCoordsRel = MinecraftWorldMathUtils.ConvertChunkCoordsAbsToRel(chunkCoords);
                if (!regionModel.Data.HasChunkGenerated(chunkCoordsRel))
                    return false;
                chunk = regionModel.Data.GetChunk(chunkCoordsRel);
                return true;
            }
            catch (Exception e)
            {
                handleChunkLoadingError(chunkCoords, e);
                return false;
            }
        }

        void handleChunkLoadingError(PointZ<int> chunkCoords, Exception e)
        {
            lock (_errorChunks)
            {
                if (_errorRegions.Contains(chunkCoords))
                    return;
                _dispatcher.InvokeAsync(() => ChunkLoadingException?.Invoke(chunkCoords, e),
                                        DispatcherPriority.Background,
                                        _reinitializingCts.Token);
                _errorChunks.Add(chunkCoords);
            }
        }

        void cleanupWorkedChunk(PointZ<int>? chunkCoords)
        {
            if (chunkCoords is null)
                return;
            lock (_workedChunks)
                _workedChunks.Remove(chunkCoords.Value);
            OnPropertyChanged(nameof(WorkedChunksCount));
        }
    }

    private RegionDataImageModel? GetRegionModelForChunk(PointZ<int> chunkCoords, out RegionStatus regionStatus)
    {
        PointZ<int> regionCoords = MinecraftWorldMathUtils.GetRegionCoordsFromChunkCoordsAbs(chunkCoords);
        lock (_loadedRegions)
            lock (_pendingRegions)
                lock (_workedRegion)
                    if (_loadedRegions.TryGetValue(regionCoords, out RegionDataImageModel? region))
                    {
                        regionStatus = RegionStatus.Loaded;
                        return region;
                    }
                    else if (_pendingRegions.Contains(regionCoords)
                            || _workedRegion.Value == regionCoords)
                    {
                        regionStatus = RegionStatus.Pending;
                        return null;
                    }
        regionStatus = RegionStatus.Missing;
        return null;
    }

    private void LoadChunk(IChunk chunk, RegionDataImageModel regionModel)
    {
        ChunkModel chunkModel = new(chunk.CoordsAbs, chunk.CoordsRel);
        _heightLevelLock.EnterReadLock();
        try
        {
            chunk.GetHighestBlockSlim(chunkModel.HighestBlocks, _heightLevel, null);
            _chunkRenderer.RenderChunk(regionModel.Image, chunk, chunkModel.HighestBlocks, _heightLevel);
        }
        finally { _heightLevelLock.ExitReadLock(); }

        chunk.Dispose();
        _redrawEvent.Set();

        lock (_renderedChunks)
            _renderedChunks.Add(chunk.CoordsAbs, chunkModel);
        OnPropertyChanged(nameof(LoadedChunksCount));
    }

    private void UnloadChunk(ChunkModel chunkModel)
    {
        _renderedChunks.Remove(chunkModel.CoordsAbs);
        chunkModel.Dispose();
    }

    private void RunRedrawLoopTask()
    {
        lock (_isRedrawTaskRunning)
        {
            if (_isRedrawTaskRunning.Value)
                return;
            _isRedrawTaskRunning.Value = true;
            _redrawTask = Task.Factory.StartNew(RedrawLoop,
                                                _stoppingCts.Token,
                                                TaskCreationOptions.LongRunning,
                                                TaskScheduler.Default);
        }
    }

    private void RedrawLoop()
    {
        try
        {
            while (!_stoppingCts.IsCancellationRequested)
            {
                _dispatcher.Invoke(() =>
                {
                    lock (_loadedRegions)
                        foreach (RegionDataImageModel region in _loadedRegions.Values)
                            region.Image.Redraw();
                    checkIfRedrawNeeded();
                }, DispatcherPriority.BackgroundHigh, _stoppingCts.Token);
                Thread.Sleep(1000 / 30);
                _redrawEvent.Wait();
            }
        }
        finally
        {
            lock (_isRedrawTaskRunning)
                _isRedrawTaskRunning.Value = false;
        }

        void checkIfRedrawNeeded()
        {
            lock (_pendingChunkLock)
                lock (_workedChunks)
                    if (_pendingChunks.Count == 0 && _workedChunks.Count == 0)
                        _redrawEvent.Reset();
        }
    }

    public BlockSlim? GetHighestBlockAt(PointZ<int> blockCoords)
    {
        // get chunk for this block
        PointZ<int> chunkCoords = MinecraftWorldMathUtils.GetChunkCoordsAbsFromBlockCoordsAbs(blockCoords);
        ChunkModel? chunk;
        lock (_renderedChunks)
            if (!_renderedChunks.TryGetValue(chunkCoords, out chunk))
                return null;
        PointZ<int> blockCoordsRel = MinecraftWorldMathUtils.ConvertBlockCoordsAbsToRelToChunk(blockCoords);
        return chunk.HighestBlocks[blockCoordsRel.X, blockCoordsRel.Z];
    }

    private void UpdateChunkHighestBlockResponsive()
    {
        lock (_renderedChunks)
            foreach (ChunkModel chunkModel in _renderedChunks.Values)
            {
                UnloadChunk(chunkModel);
                if (_visibleChunkRange.IsOutside(chunkModel.CoordsAbs))
                    continue;
                lock (_pendingChunkLock)
                {
                    _pendingChunks.Add(chunkModel.CoordsAbs);
                    _pendingChunksSet.Add(chunkModel.CoordsAbs);
                }
            }
        lock (_pendingChunkLock)
            _pendingChunks.Sort();
        RunPendingChunkTasks();
    }

    public void StartBackgroundThread()
    {
        RunRedrawLoopTask();
        RunPendingRegionTask();
        RunPendingChunkTasks();
    }

    public void StopBackgroundThread()
    {
        _stoppingCts.Cancel();
        _redrawEvent.Set();
        try
        {
            _pendingRegionTask.Wait();
            Task[] chunkLoaderTasks;
            lock (_pendingChunkTasks)
                chunkLoaderTasks = _pendingChunkTasks.Values.ToArray();
            Task.WaitAll(chunkLoaderTasks);
            _redrawTask.Wait();
        }
        catch (AggregateException e)
        {
            if (e.InnerException is not TaskCanceledException)
                throw;
        }

        _stoppingCts.Dispose();
        _stoppingCts = new CancellationTokenSource();
    }

    public void Reinitialize()
    {
        _reinitializingCts.Cancel();
        StopBackgroundThread();
        _newChunkLoaderTaskId.Value = 0;
        _reinitializingCts.Dispose();
        _reinitializingCts = new CancellationTokenSource();

        _visibleRegionRange = default;
        _visibleChunkRange = default;

        foreach (RegionDataImageModel region in _loadedRegions.Values)
            UnloadRegion(region);
        _cachedRegions.Clear();
        _pendingRegions.Clear();

        foreach (ChunkModel chunk in _renderedChunks.Values)
            UnloadChunk(chunk);
        _pendingChunks.Clear();
        _pendingChunksSet.Clear();

        _errorChunks.Clear();
        _errorRegions.Clear();

        OnPropertyChanged(nameof(VisibleRegionRange));
        OnPropertyChanged(nameof(VisibleChunkRange));
        OnPropertyChanged(nameof(LoadedRegionsCount));
        OnPropertyChanged(nameof(PendingRegionsCount));
        OnPropertyChanged(nameof(WorkedRegion));
    }

    private enum RegionStatus
    {
        Loaded,
        Pending,
        Missing
    }
}
