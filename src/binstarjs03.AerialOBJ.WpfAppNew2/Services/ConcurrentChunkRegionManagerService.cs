using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Threading;

using binstarjs03.AerialOBJ.Core;
using binstarjs03.AerialOBJ.Core.MinecraftWorld;
using binstarjs03.AerialOBJ.Core.Primitives;
using binstarjs03.AerialOBJ.Core.Threading.MessageDispatching;
using binstarjs03.AerialOBJ.WpfAppNew2.Components;
using binstarjs03.AerialOBJ.WpfAppNew2.Factories;
using binstarjs03.AerialOBJ.WpfAppNew2.Models;

namespace binstarjs03.AerialOBJ.WpfAppNew2.Services;
public class ConcurrentChunkRegionManagerService : IChunkRegionManagerService
{
    private const int s_regionBufferSize = 15;
    private readonly Random _rng = new();

    // Dependencies
    private readonly IRegionLoaderService _regionLoaderService;
    private readonly IChunkRegionManagerErrorMemoryService _crmErrorMemoryService;
    private readonly RegionImageModelFactory _regionImageModelFactory;
    private readonly IChunkRenderService _chunkRenderService;
    private readonly IMessageDispatcher _dispatcher;

    private readonly ReaderWriterLockSlimWrapper<Point2ZRange<int>> _visibleRegionRange = new() { Value = new Point2ZRange<int>() };
    private readonly ReaderWriterLockSlimWrapper<Point2ZRange<int>> _visibleChunkRange = new() { Value = new Point2ZRange<int>() };

    private readonly Dictionary<Point2Z<int>, RegionModel> _loadedRegions = new(s_regionBufferSize);
    private readonly List<Point2Z<int>> _pendingRegions = new(s_regionBufferSize);
    private readonly StructLock<Point2Z<int>?> _workedRegion = new() { Value = null };
    private Task _workedRegionTask = new(() => { });

    public ConcurrentChunkRegionManagerService(
        IRegionLoaderService regionLoaderService,
        IChunkRegionManagerErrorMemoryService chunkRegionManagerErrorMemoryService,
        IMessageDispatcherFactory messageDispatcherFactory,
        RegionImageModelFactory regionImageModelFactory,
        IChunkRenderService chunkRenderService)
    {
        _regionLoaderService = regionLoaderService;
        _crmErrorMemoryService = chunkRegionManagerErrorMemoryService;
        _regionImageModelFactory = regionImageModelFactory;
        _chunkRenderService = chunkRenderService;
        _dispatcher = messageDispatcherFactory.Create("CRM Dispatcher", ExceptionBehaviour.Ignore);
        _dispatcher.Start();
    }

    public Point2ZRange<int> VisibleRegionRange => _visibleRegionRange.Value;
    public int LoadedRegionsCount => _loadedRegions.Count;
    public int PendingRegionsCount => _pendingRegions.Count;
    public Point2Z<int>? WorkedRegion => _workedRegion.Value;
    public Point2ZRange<int> VisibleChunkRange => _visibleChunkRange.Value;

    public event Action<RegionModel>? RegionImageLoaded;
    public event Action<RegionModel>? RegionImageUnloaded;
    public event RegionReadingErrorHandler? RegionLoadingError;
    public event Action<string>? PropertyChanged;

    public void Update(Point2Z<float> cameraPos, float unitMultiplier, Size<int> screenSize)
    {
        if (RecalculateVisibleChunkRange(cameraPos, unitMultiplier, screenSize))
            if (RecalculateVisibleRegionRange())
                _dispatcher.InvokeAsynchronousNoDuplicate(ManageRegions);
    }

    public void Reinitialize()
    {
        _pendingRegions.Clear();
        foreach (RegionModel region in _loadedRegions.Values)
            UnloadRegion(region);
        _visibleRegionRange.Value = new Point2ZRange<int>();
        _workedRegion.Value = null;
        _visibleChunkRange.Value = new Point2ZRange<int>();
        _crmErrorMemoryService.Reinitialize();

        OnPropertyChanged(nameof(PendingRegionsCount));
        OnPropertyChanged(nameof(LoadedRegionsCount));
        OnPropertyChanged(nameof(VisibleRegionRange));
        OnPropertyChanged(nameof(WorkedRegion));

        OnPropertyChanged(nameof(VisibleChunkRange));
    }

    private bool RecalculateVisibleChunkRange(Point2Z<float> cameraPos, float unitMultiplier, Size<int> screenSize)
    {
        float pixelPerChunk = unitMultiplier * Section.BlockCount; // one unit (or pixel) equal to one block
        Rangeof<int> visibleChunkXRange = calculateVisibleChunkRange(screenSize.Width, pixelPerChunk, cameraPos.X);
        Rangeof<int> visibleChunkZRange = calculateVisibleChunkRange(screenSize.Height, pixelPerChunk, cameraPos.Z);

        _visibleChunkRange.Lock.EnterWriteLock();
        try
        {
            Point2ZRange<int> oldVisibleChunkRange = _visibleChunkRange.Value;
            Point2ZRange<int> newVisibleChunkRange = new(visibleChunkXRange, visibleChunkZRange);
            if (newVisibleChunkRange == oldVisibleChunkRange)
                return false;

            _visibleChunkRange.Value = newVisibleChunkRange;
        }
        finally { _visibleChunkRange.Lock.ExitWriteLock(); }
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
        _visibleChunkRange.Lock.EnterReadLock();
        try
        {
            Rangeof<int> visibleRegionXRange = calculateVisibleRegionRange(_visibleChunkRange.Value.XRange);
            Rangeof<int> visibleRegionZRange = calculateVisibleRegionRange(_visibleChunkRange.Value.ZRange);

            _visibleRegionRange.Lock.EnterWriteLock();
            try
            {
                Point2ZRange<int> oldVisibleRegionRange = _visibleRegionRange.Value;
                Point2ZRange<int> newVisibleRegionRange = new(visibleRegionXRange, visibleRegionZRange);
                if (newVisibleRegionRange == oldVisibleRegionRange)
                    return false;
                _visibleRegionRange.Value = newVisibleRegionRange;
            }
            finally { _visibleRegionRange.Lock.ExitWriteLock(); }
        }
        finally{ _visibleChunkRange.Lock.ExitReadLock(); }

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
            DispatchWorkedRegionTask();
    }

    private void UnloadCulledRegions()
    {
        // perform boundary range checking for regions outside display frame
        // unload them if outside
        _visibleRegionRange.Lock.EnterReadLock();
        try
        {
            lock (_loadedRegions)
                foreach ((Point2Z<int> regionCoords, RegionModel region) in _loadedRegions)
                    if (_visibleRegionRange.Value.IsOutside(regionCoords))
                        UnloadRegion(region);
            // remove all pending region list that is no longer visible
            _pendingRegions.RemoveAll(_visibleRegionRange.Value.IsOutside);
        }
        finally { _visibleRegionRange.Lock.ExitReadLock(); }
        OnPropertyChanged(nameof(PendingRegionsCount));
    }

    private bool QueuePendingRegions()
    {
        bool hasPendingRegion = false;
        _visibleRegionRange.Lock.EnterReadLock();
        try
        {
            for (int x = _visibleRegionRange.Value.XRange.Min; x <= _visibleRegionRange.Value.XRange.Max; x++)
                for (int z = _visibleRegionRange.Value.ZRange.Min; z <= _visibleRegionRange.Value.ZRange.Max; z++)
                {
                    Point2Z<int> regionCoords = new(x, z);
                    lock (_loadedRegions)
                        lock (_workedRegion)
                            if (_loadedRegions.ContainsKey(regionCoords)
                                || _pendingRegions.Contains(regionCoords)
                                || _workedRegion.Value == regionCoords)
                                continue;
                            else
                            {
                                _pendingRegions.Add(regionCoords);
                                hasPendingRegion = true;
                            }
                }
        }
        finally { _visibleRegionRange.Lock.ExitReadLock(); }
        OnPropertyChanged(nameof(PendingRegionsCount));
        return hasPendingRegion;
    }

    private void DispatchWorkedRegionTask(bool forced = false)
    {
        if (_pendingRegions.Count == 0)
            return;
        Point2Z<int> regionCoords = getRandomPendingRegion();

        // load region asynchronously, CRM dispatcher must be responsive to update request
        if (_workedRegionTask.Status != TaskStatus.Running || forced)
            _workedRegionTask = Task.Run(() => WorkedRegionTaskMethod(regionCoords));

        Point2Z<int> getRandomPendingRegion()
        {
            int randomIndex = _rng.Next(0, _pendingRegions.Count);
            Point2Z<int> result = _pendingRegions[randomIndex];
            _pendingRegions.RemoveAt(randomIndex);
            OnPropertyChanged(nameof(PendingRegionsCount));
            return result;
        }
    }

    // TODO WorkedRegionTaskMethod can be put on a separate class for more readability,
    // because both class can't see the private methods and fields on each other
    private void WorkedRegionTaskMethod(Point2Z<int> regionCoords)
    {
        lock (_workedRegion)
            _workedRegion.Value = regionCoords;
        OnPropertyChanged(nameof(WorkedRegion));
        try
        {
            Region? region = _regionLoaderService.LoadRegion(regionCoords, out Exception? e);
            if (region is null)
            {
                if (e is not null)
                    handleRegionLoadingError(regionCoords, e);
                return;
            }
            LoadRegion(region);
        }
        finally
        {
            lock (_workedRegion)
                _workedRegion.Value = null;
            OnPropertyChanged(nameof(WorkedRegion));
            _dispatcher.InvokeAsynchronousNoDuplicate(() => DispatchWorkedRegionTask(true));
        }

        void handleRegionLoadingError(Point2Z<int> regionCoords, Exception e)
        {
            if (!_crmErrorMemoryService.CheckHasRegionError(regionCoords))
            {
                RegionLoadingError?.Invoke(regionCoords, e);
                _crmErrorMemoryService.StoreRegionError(regionCoords);
            }
        }
    }

    private void LoadRegion(Region region)
    {
        RegionModel rim = _regionImageModelFactory.Create(region.RegionCoords, region);
        _chunkRenderService.RenderRandomNoise(rim.RegionImage,
                                              new Color()
                                              {
                                                  Alpha = 255,
                                                  Red = (byte)Random.Shared.Next(0, 255),
                                                  Green = (byte)Random.Shared.Next(0, 255),
                                                  Blue = (byte)Random.Shared.Next(0, 255),
                                              },
                                              64);
        rim.RegionImage.Redraw();
        _visibleRegionRange.Lock.EnterReadLock();
        try
        {
            lock (_loadedRegions)
            {
                if (_visibleRegionRange.Value.IsOutside(region.RegionCoords)
                    || _loadedRegions.ContainsKey(region.RegionCoords))
                    return;
                App.Current.Dispatcher.InvokeAsync(() => RegionImageLoaded?.Invoke(rim), DispatcherPriority.Render);
                _loadedRegions.Add(region.RegionCoords, rim);
            }
        }
        finally { _visibleRegionRange.Lock.ExitReadLock(); }
        OnPropertyChanged(nameof(LoadedRegionsCount));
    }

    private void UnloadRegion(RegionModel region)
    {
        lock (_loadedRegions)
        {
            App.Current.Dispatcher.InvokeAsync(() => RegionImageUnloaded?.Invoke(region), DispatcherPriority.Render);
            _loadedRegions.Remove(region.RegionCoords);
        }
        OnPropertyChanged(nameof(LoadedRegionsCount));
    }

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(propertyName);
    }
}
