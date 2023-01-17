using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using binstarjs03.AerialOBJ.Core;
using binstarjs03.AerialOBJ.Core.MinecraftWorld;
using binstarjs03.AerialOBJ.Core.Primitives;
using binstarjs03.AerialOBJ.WpfApp.Components;
using binstarjs03.AerialOBJ.WpfApp.Factories;
using binstarjs03.AerialOBJ.WpfApp.Models;
using binstarjs03.AerialOBJ.WpfApp.Services.ChunkRendering;
using binstarjs03.AerialOBJ.WpfApp.Services.Dispatcher;
using binstarjs03.AerialOBJ.WpfApp.Services.IOService;

using CommunityToolkit.Mvvm.ComponentModel;

namespace binstarjs03.AerialOBJ.WpfApp.Services;

[ObservableObject]
public partial class ChunkRegionManager : IChunkRegionManager
{
    private const int s_initialRegionBufferSize = 15;
    private const int s_initialChunkBufferSize = s_initialRegionBufferSize * IRegion.TotalChunkCount;
    private readonly Random _random = new();

    // Dependencies -----------------------------------------------------------
    private readonly IRegionDiskLoader _regionProvider;
    private readonly RegionDataImageModelFactory _regionDataImageModelFactory;
    private readonly IRegionImagePooler _regionImagePooler;
    private readonly IChunkRenderer _chunkRenderer;
    private readonly IDispatcher _dispatcher;

    // Threadings -------------------------------------------------------------
    private CancellationTokenSource _stoppingCts = new(); // use this when pausing
    private CancellationTokenSource _reinitializingCts = new(); // use this when reinitializing
    private readonly ManualResetEventSlim _updateChunkHighestBlockEvent = new(true);

    private readonly ReferenceWrap<bool> _isPendingRegionTaskRunning = new() { Value = false };
    private Task _pendingRegionTask = Task.CompletedTask;

    private readonly ReferenceWrap<bool> _isRedrawTaskRunning = new() { Value = false };
    private Task _redrawTask = Task.CompletedTask;

    private readonly int _pendingChunkTasksLimit = Math.Max(1, 8);
    private readonly Dictionary<uint, Task> _pendingChunkTasks = new(Environment.ProcessorCount);
    private readonly ReferenceWrap<uint> _newChunkLoaderTaskId = new() { Value = default };

    // Manager States ---------------------------------------------------------
    private readonly List<Point2Z<int>> _errorRegions = new();
    private readonly HashSet<Point2Z<int>> _errorChunks = new();
    private int _heightLevel;
    private readonly ReaderWriterLockSlim _heightLevelLock = new();

    private Point2ZRange<int> _visibleRegionRange = default;
    private Point2ZRange<int> _visibleChunkRange = default;
    private readonly ReaderWriterLockSlim _visibleRegionRangeLock = new();
    private readonly ReaderWriterLockSlim _visibleChunkRangeLock = new();

    private readonly Dictionary<Point2Z<int>, RegionDataImageModel> _loadedRegions = new(s_initialRegionBufferSize);
    private readonly Dictionary<Point2Z<int>, RegionDataImageModel> _cachedRegions = new(s_initialRegionBufferSize);
    private readonly List<Point2Z<int>> _pendingRegions = new(s_initialRegionBufferSize);
    private readonly ReferenceWrap<Point2Z<int>?> _workedRegion = new() { Value = null };

    private readonly Dictionary<Point2Z<int>, ChunkModel> _loadedChunks = new(s_initialChunkBufferSize);
    private readonly HashSet<Point2Z<int>> _pendingChunksSet = new(s_initialChunkBufferSize);
    private readonly List<Point2Z<int>> _pendingChunks = new(s_initialChunkBufferSize);
    private readonly object _pendingChunkLock = new();
    private readonly List<Point2Z<int>> _workedChunks = new(Environment.ProcessorCount);

    public ChunkRegionManager(
        IRegionDiskLoader regionProvider,
        RegionDataImageModelFactory regionImageModelFactory,
        IRegionImagePooler regionImagePooler,
        IChunkRenderer chunkRenderer,
        IDispatcher dispatcher)
    {
        _regionProvider = regionProvider;
        _regionDataImageModelFactory = regionImageModelFactory;
        _regionImagePooler = regionImagePooler;
        _chunkRenderer = chunkRenderer;
        _dispatcher = dispatcher;
    }

    public Point2ZRange<int> VisibleRegionRange => _visibleRegionRange;
    public int VisibleRegionsCount => VisibleRegionRange.Sum;
    public int LoadedRegionsCount => _loadedRegions.Count;
    public int CachedRegionsCount => _cachedRegions.Count;
    public int PendingRegionsCount => _pendingRegions.Count;
    public Point2Z<int>? WorkedRegion => _workedRegion.Value;

    public Point2ZRange<int> VisibleChunkRange => _visibleChunkRange;
    public int VisibleChunksCount => VisibleChunkRange.Sum;
    public int LoadedChunksCount => _loadedChunks.Count;
    public int PendingChunksCount => _pendingChunks.Count;
    public int WorkedChunksCount => _workedChunks.Count;

    public event Action<RegionDataImageModel>? RegionLoaded;
    public event Action<RegionDataImageModel>? RegionUnloaded;
    public event ChunkRegionReadingErrorHandler? RegionLoadingException;
    public event ChunkRegionReadingErrorHandler? ChunkLoadingException;

    public void Update(Point2Z<float> cameraPos, float unitMultiplier, Size<int> screenSize)
    {
        if (IsVisibleChunkRangeChanged(cameraPos, unitMultiplier, screenSize))
        {
            if (IsVisibleRegionRangeChanged())
                ManageRegions();
            ManageChunks();
        }
    }

    public void UpdateHeightLevel(int heightLevel, HeightSliderSetting setting)
    {
        _heightLevelLock.EnterWriteLock();
        try
        {
            if (_heightLevel != heightLevel)
                _heightLevel = heightLevel;
            if (setting == HeightSliderSetting.Responsive)
                UpdateChunkHighestBlockResponsive();
            else
                UpdateChunkHighestBlockBlocking();
        }
        finally { _heightLevelLock.ExitWriteLock(); }
    }

    private bool IsVisibleChunkRangeChanged(Point2Z<float> cameraPos, float unitMultiplier, Size<int> screenSize)
    {
        _visibleChunkRangeLock.EnterWriteLock();
        try
        {
            Point2ZRange<int> oldVisibleChunkRange = _visibleChunkRange;
            Point2ZRange<int> newVisibleChunkRange = CalculateVisibleChunkRange(cameraPos, unitMultiplier, screenSize);
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
            Point2ZRange<int> oldVisibleRegionRange = _visibleRegionRange;
            Point2ZRange<int> newVisibleRegionRange = CalculateVisibleRegionRange(_visibleChunkRange);
            if (newVisibleRegionRange == oldVisibleRegionRange)
                return false;
            _visibleRegionRange = newVisibleRegionRange;
            OnPropertyChanged(nameof(VisibleRegionRange));
            OnPropertyChanged(nameof(VisibleRegionsCount));
            return true;
        }
        finally { _visibleRegionRangeLock.ExitWriteLock(); }
    }

    private static Point2ZRange<int> CalculateVisibleChunkRange(Point2Z<float> cameraPos, float unitMultiplier, Size<int> screenSize)
    {
        float pixelPerChunk = unitMultiplier * IChunk.BlockCount; // one unit (or pixel) equal to one block
        Rangeof<int> visibleChunkXRange = calculateSingleAxis(screenSize.Width, pixelPerChunk, cameraPos.X);
        Rangeof<int> visibleChunkZRange = calculateSingleAxis(screenSize.Height, pixelPerChunk, cameraPos.Z);
        return new Point2ZRange<int>(visibleChunkXRange, visibleChunkZRange);

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

    private static Point2ZRange<int> CalculateVisibleRegionRange(Point2ZRange<int> visibleChunkRange)
    {
        Rangeof<int> visibleRegionXRange = calculateSingleAxis(visibleChunkRange.XRange);
        Rangeof<int> visibleRegionZRange = calculateSingleAxis(visibleChunkRange.ZRange);
        return new Point2ZRange<int>(visibleRegionXRange, visibleRegionZRange);

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
            foreach ((Point2Z<int> regionCoords, RegionDataImageModel regionModel) in _loadedRegions)
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
                // load cached region instantly without having to go through pending queue
                Point2Z<int> regionCoords = new(x, z);
                lock (_cachedRegions)
                    if (_cachedRegions.TryGetValue(regionCoords, out RegionDataImageModel? regionDataImageModel))
                    {
                        LoadRegion(regionDataImageModel, true);
                        continue;
                    }
                lock (_loadedRegions)
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
            _pendingRegionTask = new Task(ProcessPendingRegion,
                                          _stoppingCts.Token,
                                          TaskCreationOptions.LongRunning);
            _isPendingRegionTaskRunning.Value = true;
            _pendingRegionTask.Start();
        }
    }

    private void ProcessPendingRegion()
    {
        try
        {
            while (!_stoppingCts.IsCancellationRequested)
            {
                // get random pending region then assign it to worked region
                if (!tryGetRandomPendingRegion(out Point2Z<int> regionCoords))
                    return;

                // update worked region, this is to prevent adding current coords to pending again
                setWorkedRegionTo(regionCoords);

                // get region and load it if succeed
                if (tryGetRegion(regionCoords, out RegionDataImageModel? region))
                    LoadRegion(region);
                setWorkedRegionTo(null);
            }
        }
        finally
        {
            setWorkedRegionTo(null);
            lock (_isPendingRegionTaskRunning)
                _isPendingRegionTaskRunning.Value = false;
        }

        bool tryGetRandomPendingRegion(out Point2Z<int> result)
        {
            result = new Point2Z<int>();
            lock (_pendingRegions)
            {
                if (_pendingRegions.Count == 0)
                    return false;
                int randomIndex = _random.Next(0, _pendingRegions.Count);
                result = _pendingRegions[randomIndex];
                _pendingRegions.RemoveAt(randomIndex);
            }
            OnPropertyChanged(nameof(PendingRegionsCount));
            return true;
        }

        bool tryGetRegion(Point2Z<int> regionCoords, [NotNullWhen(true)] out RegionDataImageModel? regionDataImageModel)
        {
            regionDataImageModel = null;

            // try get region from cache, else proceed to get it from disk
            lock (_cachedRegions)
                if (_cachedRegions.TryGetValue(regionCoords, out regionDataImageModel))
                    return true;
            try
            {
                if (_regionProvider.TryGetRegion(regionCoords, _reinitializingCts.Token, out IRegion? region))
                {
                    regionDataImageModel = _regionDataImageModelFactory.Create(region, _reinitializingCts.Token);
                    lock (_cachedRegions)
                        _cachedRegions.Add(regionCoords, regionDataImageModel);
                    OnPropertyChanged(nameof(CachedRegionsCount));
                    //_chunkRenderer.RenderRandomNoise(regionDataImageModel.Image, Random.Shared.NextColor(), 64);
                    //regionDataImageModel.Image.Redraw();
                    return true;
                }
            }
            catch (TaskCanceledException) { }
            catch (Exception e) { handleException(regionCoords, e); }
            return false;
        }

        void handleException(Point2Z<int> regionCoords, Exception e)
        {
            if (_errorRegions.Contains(regionCoords))
                return;
            _dispatcher.InvokeAsync(() => RegionLoadingException?.Invoke(regionCoords, e),
                                    DispatcherPriority.Background,
                                    _reinitializingCts.Token);
            _errorRegions.Add(regionCoords);
        }

        void setWorkedRegionTo(Point2Z<int>? regionCoords)
        {
            lock (_workedRegion)
                _workedRegion.Value = regionCoords;
            OnPropertyChanged(nameof(WorkedRegion));
        }
    }

    private void LoadRegion(RegionDataImageModel region, bool isMainThread = false)
    {
        if (isMainThread)
            loadRegion();
        else
            _dispatcher.Invoke(loadRegion, DispatcherPriority.Background, _reinitializingCts.Token);
        OnPropertyChanged(nameof(LoadedRegionsCount));

        void loadRegion()
        {
            lock (_loadedRegions)
            {
                if (_visibleRegionRange.IsOutside(region.Data.Coords)
                    || _loadedRegions.ContainsKey(region.Data.Coords)
                    || _stoppingCts.IsCancellationRequested)
                    return;
                _loadedRegions.Add(region.Data.Coords, region);
                RegionLoaded?.Invoke(region);
            }
        }
    }

    private void UnloadRegion(RegionDataImageModel regionModel)
    {
        // locking is already done before this method called
        _loadedRegions.Remove(regionModel.Data.Coords);
        RegionUnloaded?.Invoke(regionModel);
        SwapRegionImage(regionModel);
    }

    // swap dirty (has rendered chunks) region image with new one
    private void SwapRegionImage(RegionDataImageModel regionModel)
    {
        _regionImagePooler.Return(regionModel.Image);
        regionModel.Image = _regionImagePooler.Rent(regionModel.Data.Coords, _reinitializingCts.Token);
    }

    private void ManageChunks()
    {
        UnloadCulledChunks();
        QueuePendingChunks();
        RunPendingChunkTasks();
        OnPropertyChanged(nameof(LoadedChunksCount));
        OnPropertyChanged(nameof(PendingChunksCount));
    }

    private void UnloadCulledChunks()
    {
        // perform boundary range checking for regions outside display frame
        // unload loaded / cancel pending chunk if outside
        lock (_loadedChunks)
            foreach ((Point2Z<int> chunkCoords, ChunkModel chunk) in _loadedChunks)
                if (_visibleChunkRange.IsOutside(chunkCoords))
                    UnloadChunk(chunk);
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
                Point2Z<int> chunkCoords = new(x, z);
                lock (_loadedChunks)
                    lock (_pendingChunkLock)
                        lock (_workedChunks)
                        {
                            if (_loadedChunks.ContainsKey(chunkCoords)
                                || _pendingChunksSet.Contains(chunkCoords)
                                || _workedChunks.Contains(chunkCoords))
                                continue;
                            else
                            {
                                _pendingChunks.Add(chunkCoords);
                                _pendingChunksSet.Add(chunkCoords);
                            }
                        }
            }
    }

    // TODO we may be able to create new class that manages and track pool of tasks
    private void RunPendingChunkTasks()
    {
        lock (_pendingChunkTasks)
        {
            while (_pendingChunkTasks.Count < _pendingChunkTasksLimit)
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
            while (!_stoppingCts.IsCancellationRequested)
            {
                // get random pending chunk then assign it to worked chunk
                if (!tryGetRandomPendingChunk(out Point2Z<int> chunkCoords))
                    return;
                lock (_workedChunks)
                    _workedChunks.Add(chunkCoords);
                OnPropertyChanged(nameof(WorkedChunksCount));

                // get both chunk and region
                (IChunk? chunk, RegionDataImageModel? region) = getChunkAndRegion(chunkCoords);
                if (chunk is null || region is null)
                {
                    cleanupWorkedChunk(chunkCoords);
                    continue;
                }

                // load it
                LoadChunk(chunk, region);
                cleanupWorkedChunk(chunkCoords);
            }
        }
        finally
        {
            lock (_pendingChunkTasks)
                _pendingChunkTasks.Remove(taskId);
        }

        bool tryGetRandomPendingChunk(out Point2Z<int> result)
        {
            result = new Point2Z<int>();
            lock (_pendingChunkLock)
            {
                if (_pendingChunks.Count == 0)
                    return false;
                int randomIndex = _random.Next(0, _pendingChunks.Count);
                result = _pendingChunks[randomIndex];
                _pendingChunks.RemoveAt(randomIndex);
                _pendingChunksSet.Remove(result);
            }
            OnPropertyChanged(nameof(PendingChunksCount));
            return true;
        }

        (IChunk? chunk, RegionDataImageModel? region) getChunkAndRegion(Point2Z<int> chunkCoords)
        {
            // get the underlying region
            RegionDataImageModel? region = GetRegionModelForChunk(chunkCoords, out RegionStatus status);
            if (region is null)
            {
                // can't get region because it isn't loaded yet,
                // put chunk back to pending list and "lets hope" in next encounter,
                // region is loaded. We may refactor this to make it more optimal
                if (status == RegionStatus.Worked || status == RegionStatus.Pending)
                    lock (_pendingChunkLock)
                    {
                        _pendingChunks.Add(chunkCoords);
                        _pendingChunksSet.Add(chunkCoords);
                    }
                OnPropertyChanged(nameof(PendingChunksCount));
                return (null, null);
            }
            Point2Z<int> chunkCoordsRel = MinecraftWorldMathUtils.ConvertChunkCoordsAbsToRel(chunkCoords);
            try
            {
                if (!region.Data.HasChunkGenerated(chunkCoordsRel))
                    return (null, null);
                return (region.Data.GetChunk(chunkCoordsRel), region);
            }
            catch (Exception e)
            {
                handleChunkLoadingError(chunkCoords, e);
                return (null, null);
            }
        }

        void handleChunkLoadingError(Point2Z<int> chunkCoords, Exception e)
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

        void cleanupWorkedChunk(Point2Z<int> chunkCoords)
        {
            lock (_workedChunks)
                _workedChunks.Remove(chunkCoords);
            OnPropertyChanged(nameof(WorkedChunksCount));
        }
    }

    private RegionDataImageModel? GetRegionModelForChunk(Point2Z<int> chunkCoords, out RegionStatus regionStatus)
    {
        Point2Z<int> regionCoords = MinecraftWorldMathUtils.GetRegionCoordsFromChunkCoordsAbs(chunkCoords);
        lock (_loadedRegions)
            lock (_pendingRegions)
                lock (_workedRegion)
                    if (_loadedRegions.TryGetValue(regionCoords, out RegionDataImageModel? region))
                    {
                        regionStatus = RegionStatus.Loaded;
                        return region;
                    }
                    else if (_workedRegion.Value == regionCoords)
                    {
                        regionStatus = RegionStatus.Worked;
                        return null;
                    }
                    else if (_pendingRegions.Contains(regionCoords))
                    {
                        regionStatus = RegionStatus.Pending;
                        return null;
                    }
        regionStatus = RegionStatus.Missing;
        return null;
    }

    private void LoadChunk(IChunk chunkData, RegionDataImageModel regionModel)
    {
        _updateChunkHighestBlockEvent.Wait();
        ChunkModel chunk = new() { Data = chunkData };
        _heightLevelLock.EnterReadLock();
        try
        {
            chunk.LoadHighestBlock(_heightLevel);
        }
        finally { _heightLevelLock.ExitReadLock(); }
        _chunkRenderer.RenderChunk(regionModel.Image, chunk.HighestBlocks, chunk.Data.CoordsRel);
        _visibleChunkRangeLock.EnterReadLock();
        try
        {
            lock (_loadedChunks)
            {
                if (_visibleChunkRange.IsOutside(chunkData.CoordsAbs)
                    || _loadedChunks.ContainsKey(chunkData.CoordsAbs)
                    || _reinitializingCts.IsCancellationRequested)
                    return;
                _loadedChunks.Add(chunkData.CoordsAbs, chunk);
            }
            OnPropertyChanged(nameof(LoadedChunksCount));
        }
        finally { _visibleChunkRangeLock.ExitReadLock(); }
    }

    private void UnloadChunk(ChunkModel chunk)
    {
        _loadedChunks.Remove(chunk.Data.CoordsAbs);
        // before returning, we want to erase region image part for this chunk,
        // but if region is not loaded, well, just move on cause it doesn't even exist
        RegionDataImageModel? region = GetRegionModelForChunk(chunk.Data.CoordsAbs, out _);
        if (region is not null)
            _chunkRenderer.EraseChunk(region.Image, chunk.Data.CoordsRel);
        chunk.Dispose();
    }

    private void RunRedrawLoopTask()
    {
        lock (_isRedrawTaskRunning)
        {
            if (_isRedrawTaskRunning.Value)
                return;
            _redrawTask = new Task(RedrawLoop,
                                   _stoppingCts.Token,
                                   TaskCreationOptions.LongRunning);
            _isRedrawTaskRunning.Value = true;
            _redrawTask.Start();
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
                }, DispatcherPriority.BackgroundHigh, _stoppingCts.Token);
                Thread.Sleep(1000 / 30);
            }
        }
        finally
        {
            lock (_isRedrawTaskRunning)
                _isRedrawTaskRunning.Value = false;
        }
    }

    public BlockSlim? GetHighestBlockAt(Point2Z<int> blockCoords)
    {
        // get chunk for this block
        Point2Z<int> chunkCoords = MinecraftWorldMathUtils.GetChunkCoordsAbsFromBlockCoordsAbs(blockCoords);
        ChunkModel? chunk;
        lock (_loadedChunks)
            if (!_loadedChunks.TryGetValue(chunkCoords, out chunk))
                return null;
        Point2Z<int> blockCoordsRel = MinecraftWorldMathUtils.ConvertBlockCoordsAbsToRelToChunk(blockCoords);
        return chunk.HighestBlocks[blockCoordsRel.X, blockCoordsRel.Z];
    }

    private void UpdateChunkHighestBlockResponsive()
    {
        lock (_loadedChunks)
            foreach (var item in _loadedChunks.Values)
            {
                UnloadChunk(item);
                lock (_pendingChunkLock)
                {
                    _pendingChunks.Add(item.Data.CoordsAbs);
                    _pendingChunksSet.Add(item.Data.CoordsAbs);
                }
            }
        RunPendingChunkTasks();
    }

    private void UpdateChunkHighestBlockBlocking()
    {
        _updateChunkHighestBlockEvent.Reset();
        try
        {
            lock (_loadedChunks)
            {
                Queue<ChunkModel> chunks = new(_loadedChunks.Values);
                if (chunks.Count == 0)
                    return;
                Task[] tasks = new Task[Math.Clamp(Environment.ProcessorCount, 0, chunks.Count)];
                for (int i = 0; i < tasks.Length; i++)
                {
                    tasks[i] = new Task(() => updateChunkHighestBlock(chunks));
                    tasks[i].Start();
                }
                Task.WaitAll(tasks);
            }
        }
        finally
        {
            _updateChunkHighestBlockEvent.Set();
        }

        void updateChunkHighestBlock(Queue<ChunkModel> chunkModels)
        {
            while (true)
            {
                ChunkModel? chunkModel;
                lock (chunkModels)
                    // stop when all chunks are updated (empty queue)
                    if (!chunkModels.TryDequeue(out chunkModel))
                        return;
                chunkModel.LoadHighestBlock(_heightLevel);
                RegionDataImageModel? regionDataImageModel = GetRegionModelForChunk(chunkModel.Data.CoordsAbs, out _);
                if (regionDataImageModel is null)
                    continue;
                _chunkRenderer.RenderChunk(regionDataImageModel.Image, chunkModel.HighestBlocks, chunkModel.Data.CoordsRel);
            }
        }
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
        _pendingRegionTask.Wait();
        Task[] chunkLoaderTasks;
        lock (_pendingChunkTasks)
            chunkLoaderTasks = _pendingChunkTasks.Values.ToArray();
        Task.WaitAll(chunkLoaderTasks);
        _redrawTask.Wait();

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

        foreach (ChunkModel chunk in _loadedChunks.Values)
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
        Worked,
        Missing
    }
}
