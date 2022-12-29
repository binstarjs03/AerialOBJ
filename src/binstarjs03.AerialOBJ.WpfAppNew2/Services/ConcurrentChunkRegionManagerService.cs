using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

using binstarjs03.AerialOBJ.Core;
using binstarjs03.AerialOBJ.Core.MinecraftWorld;
using binstarjs03.AerialOBJ.Core.Primitives;
using binstarjs03.AerialOBJ.WpfAppNew2.Components;
using binstarjs03.AerialOBJ.WpfAppNew2.Factories;
using binstarjs03.AerialOBJ.WpfAppNew2.Models;

namespace binstarjs03.AerialOBJ.WpfAppNew2.Services;
public class ConcurrentChunkRegionManagerService : IChunkRegionManagerService
{
    private const int s_regionBufferSize = 15;
    private readonly Random _rng = new();

    // Dependencies -----------------------------------------------------------
    private readonly IRegionLoaderService _regionLoaderService;
    private readonly IChunkRegionManagerErrorMemoryService _crmErrorMemoryService;
    private readonly RegionImageModelFactory _regionModelFactory;
    private readonly IChunkRenderService _chunkRenderService;

    // Threadings -------------------------------------------------------------
    private CancellationTokenSource _cts = new();
    private readonly int _chunkLoaderTasksLimit = Environment.ProcessorCount;

    private readonly StructLock<bool> _isRegionLoaderTaskRunning = new() { Value = false };
    private Task _regionLoaderTask = new(() => { });

    private readonly Dictionary<uint, Task> _chunkLoaderTasks = new(Environment.ProcessorCount);
    private readonly StructLock<uint> _newChunkLoaderTaskId = new() { Value = 0 };

    // Manager States ---------------------------------------------------------
    private readonly StructLock<Point2ZRange<int>> _visibleRegionRange = new() { Value = new Point2ZRange<int>() };
    private readonly StructLock<Point2ZRange<int>> _visibleChunkRange = new() { Value = new Point2ZRange<int>() };

    private readonly Dictionary<Point2Z<int>, RegionModel> _loadedRegions = new(s_regionBufferSize);
    private readonly List<Point2Z<int>> _pendingRegions = new(s_regionBufferSize);
    private readonly StructLock<Point2Z<int>?> _workedRegion = new() { Value = null };


    public ConcurrentChunkRegionManagerService(
        IRegionLoaderService regionLoaderService,
        IChunkRegionManagerErrorMemoryService chunkRegionManagerErrorMemoryService,
        RegionImageModelFactory regionImageModelFactory,
        IChunkRenderService chunkRenderService)
    {
        _regionLoaderService = regionLoaderService;
        _crmErrorMemoryService = chunkRegionManagerErrorMemoryService;
        _regionModelFactory = regionImageModelFactory;
        _chunkRenderService = chunkRenderService;
    }

    public Point2ZRange<int> VisibleRegionRange => _visibleRegionRange.Value;
    public int LoadedRegionsCount => _loadedRegions.Count;
    public int PendingRegionsCount => _pendingRegions.Count;
    public Point2Z<int>? WorkedRegion => _workedRegion.Value;
    public Point2ZRange<int> VisibleChunkRange => _visibleChunkRange.Value;

    public event Action<string>? PropertyChanged;
    public event Action<RegionModel>? RegionImageLoaded;
    public event Action<RegionModel>? RegionImageUnloaded;
    public event ChunkRegionReadingErrorHandler? RegionLoadingError;
    public event ChunkRegionReadingErrorHandler? ChunkLoadingError;

    public void Update(Point2Z<float> cameraPos, float unitMultiplier, Size<int> screenSize)
    {
        if (RecalculateVisibleChunkRange(cameraPos, unitMultiplier, screenSize))
            if (RecalculateVisibleRegionRange())
                ManageRegions();
        //ManageChunks();
    }

    public void Reinitialize()
    {
        _cts.Cancel();
        _regionLoaderTask.Wait();

        _visibleRegionRange.Value = new Point2ZRange<int>();
        _visibleChunkRange.Value = new Point2ZRange<int>();
        foreach (RegionModel region in _loadedRegions.Values)
            UnloadRegion(region);
        _pendingRegions.Clear();
        _crmErrorMemoryService.Reinitialize();
        _regionLoaderService.PurgeCache();

        _cts.Dispose();
        _cts = new CancellationTokenSource();

        OnPropertyChanged(nameof(VisibleRegionRange));
        OnPropertyChanged(nameof(VisibleChunkRange));
        OnPropertyChanged(nameof(LoadedRegionsCount));
        OnPropertyChanged(nameof(PendingRegionsCount));
        OnPropertyChanged(nameof(WorkedRegion));
    }

    private bool RecalculateVisibleChunkRange(Point2Z<float> cameraPos, float unitMultiplier, Size<int> screenSize)
    {
        float pixelPerChunk = unitMultiplier * Section.BlockCount; // one unit (or pixel) equal to one block
        Rangeof<int> visibleChunkXRange = calculateVisibleChunkRange(screenSize.Width, pixelPerChunk, cameraPos.X);
        Rangeof<int> visibleChunkZRange = calculateVisibleChunkRange(screenSize.Height, pixelPerChunk, cameraPos.Z);

        lock (_visibleChunkRange)
        {
            Point2ZRange<int> oldVisibleChunkRange = _visibleChunkRange.Value;
            Point2ZRange<int> newVisibleChunkRange = new(visibleChunkXRange, visibleChunkZRange);
            if (newVisibleChunkRange == oldVisibleChunkRange)
                return false;

            _visibleChunkRange.Value = newVisibleChunkRange;
        }
        OnPropertyChanged(nameof(VisibleChunkRange));
        return true;

        static Rangeof<int> calculateVisibleChunkRange(int screenSize, float pixelPerChunk, float cameraPos)
        {
            // Camera chunk in here means which chunk the camera is pointing to.
            // Floating point accuracy is crucial in here
            float cameraChunk = cameraPos / Section.BlockCount;

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

    private bool RecalculateVisibleRegionRange()
    {
        lock (_visibleChunkRange)
        {
            Rangeof<int> visibleRegionXRange = calculateVisibleRegionRange(_visibleChunkRange.Value.XRange);
            Rangeof<int> visibleRegionZRange = calculateVisibleRegionRange(_visibleChunkRange.Value.ZRange);

            lock (_visibleRegionRange)
            {
                Point2ZRange<int> oldVisibleRegionRange = _visibleRegionRange.Value;
                Point2ZRange<int> newVisibleRegionRange = new(visibleRegionXRange, visibleRegionZRange);
                if (newVisibleRegionRange == oldVisibleRegionRange)
                    return false;
                _visibleRegionRange.Value = newVisibleRegionRange;
            }
        }

        OnPropertyChanged(nameof(VisibleRegionRange));
        return true;

        static Rangeof<int> calculateVisibleRegionRange(Rangeof<int> visibleChunkRange)
        {
            // Calculating region range is easier since we only need to
            // divide the range by how many chunks in region (in single axis)
            int regionMinX = MathUtils.DivFloor(visibleChunkRange.Min, Region.ChunkCount);
            int regionMaxX = MathUtils.DivFloor(visibleChunkRange.Max, Region.ChunkCount);
            return new Rangeof<int>(regionMinX, regionMaxX);
        }
    }

    private void ManageRegions()
    {
        UnloadCulledRegions();
        if (QueuePendingRegions())
            RunTaskNoDuplicate(LoadPendingRegions, ref _regionLoaderTask, _isRegionLoaderTaskRunning);
    }

    private void UnloadCulledRegions()
    {
        // perform boundary range checking for regions outside display frame
        // unload loaded / cancel pending region if outside
        lock (_visibleRegionRange)
        {
            lock (_loadedRegions)
                foreach ((Point2Z<int> regionCoords, RegionModel region) in _loadedRegions)
                    if (_visibleRegionRange.Value.IsOutside(regionCoords))
                        UnloadRegion(region);
            // remove all pending region list that is no longer visible
            lock (_pendingRegions)
                _pendingRegions.RemoveAll(_visibleRegionRange.Value.IsOutside);
        }
        OnPropertyChanged(nameof(PendingRegionsCount));
    }

    private bool QueuePendingRegions()
    {
        // this section of code may looks slow but mostly
        // only few regions are visible to the screen at a time
        lock (_visibleRegionRange)
        {
            for (int x = _visibleRegionRange.Value.XRange.Min; x <= _visibleRegionRange.Value.XRange.Max; x++)
                for (int z = _visibleRegionRange.Value.ZRange.Min; z <= _visibleRegionRange.Value.ZRange.Max; z++)
                {
                    Point2Z<int> regionCoords = new(x, z);
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
        OnPropertyChanged(nameof(PendingRegionsCount));
        lock (_pendingRegions)
            return _pendingRegions.Count > 0;
    }


    private void LoadPendingRegions()
    {
        try
        {
            while (!_cts.IsCancellationRequested)
            {
                // get random pending region then assign it to worked region
                if (!tryGetRandomPendingRegion(out Point2Z<int> regionCoords))
                    return;

                // update worked region, this is to prevent adding current coords to pending again
                lock (_workedRegion)
                    _workedRegion.Value = regionCoords;
                OnPropertyChanged(nameof(WorkedRegion));

                // get region
                Region? region = tryGetRegion(regionCoords);
                if (region is null)
                {
                    cleanupWorkedRegion();
                    continue;
                }

                // load it
                LoadRegion(region);
                cleanupWorkedRegion();
            }
        }
        finally { cleanupWorkedRegion(); }

        bool tryGetRandomPendingRegion(out Point2Z<int> result)
        {
            result = new Point2Z<int>();
            lock (_pendingRegions)
            {
                if (_pendingRegions.Count == 0)
                    return false;
                int randomIndex = _rng.Next(0, _pendingRegions.Count);
                result = _pendingRegions[randomIndex];
                _pendingRegions.RemoveAt(randomIndex);
            }
            OnPropertyChanged(nameof(PendingRegionsCount));
            return true;
        }

        Region? tryGetRegion(Point2Z<int> regionCoords)
        {
            Region? region = _regionLoaderService.LoadRegion(regionCoords, _cts.Token, out Exception? e);
            if (region is null)
            {
                if (e is not null)
                    handleRegionLoadingError(regionCoords, e);
                return null;
            }
            return region;
        }

        void handleRegionLoadingError(Point2Z<int> regionCoords, Exception e)
        {
            if (_crmErrorMemoryService.CheckHasRegionError(regionCoords))
                return;
            App.Current.Dispatcher.InvokeAsync(() => RegionLoadingError?.Invoke(regionCoords, e));
            _crmErrorMemoryService.StoreRegionError(regionCoords);
        }

        void cleanupWorkedRegion()
        {
            lock (_workedRegion)
                _workedRegion.Value = null;
            OnPropertyChanged(nameof(WorkedRegion));
        }
    }

    private void LoadRegion(Region region)
    {
        RegionModel regionModel;
        try
        {
            regionModel = _regionModelFactory.Create(region.RegionCoords, region, _cts.Token);
        }
        catch (TaskCanceledException)
        {
            return;
        }
        _chunkRenderService.RenderRandomNoise(regionModel.RegionImage,
                                              new Color()
                                              {
                                                  Alpha = 255,
                                                  Red = (byte)Random.Shared.Next(0, 255),
                                                  Green = (byte)Random.Shared.Next(0, 255),
                                                  Blue = (byte)Random.Shared.Next(0, 255),
                                              },
                                              64);
        regionModel.RegionImage.Redraw();
        lock (_visibleRegionRange)
            lock (_loadedRegions)
            {
                if (_visibleRegionRange.Value.IsOutside(region.RegionCoords)
                    || _loadedRegions.ContainsKey(region.RegionCoords)
                    || _cts.IsCancellationRequested)
                    return;
                App.Current.Dispatcher.InvokeAsync(() => RegionImageLoaded?.Invoke(regionModel), DispatcherPriority.Render);
                _loadedRegions.Add(region.RegionCoords, regionModel);
            }
        OnPropertyChanged(nameof(LoadedRegionsCount));
    }

    private void UnloadRegion(RegionModel region)
    {
        // locking is already done before this method called
        if (App.Current.CheckAccess())
            RegionImageUnloaded?.Invoke(region);
        else
            App.Current.Dispatcher.InvokeAsync(() => RegionImageUnloaded?.Invoke(region), DispatcherPriority.Render);
        _loadedRegions.Remove(region.RegionCoords);
        OnPropertyChanged(nameof(LoadedRegionsCount));
    }


    // TODO we may be able to move out these two methods into separate class
    // since the definition is about threading and its out of this class responsibility scope
    // This is to conform SRP
    private static void RunTaskNoDuplicate(Action method, ref Task task, StructLock<bool> isTaskRunning)
    {
        lock (isTaskRunning)
        {
            if (isTaskRunning.Value)
                return;
            isTaskRunning.Value = true;
            task = Task.Run(() => RunTaskMethodWrapper(method, isTaskRunning));
        }

    }

    private static void RunTaskMethodWrapper(Action method, StructLock<bool> isTaskRunning)
    {
        try
        {
            method();
        }
        finally
        {
            lock (isTaskRunning)
                isTaskRunning.Value = false;
        }
    }

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(propertyName);
    }
}
