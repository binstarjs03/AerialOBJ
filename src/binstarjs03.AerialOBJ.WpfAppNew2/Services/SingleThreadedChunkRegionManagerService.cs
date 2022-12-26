using System;
using System.Collections.Generic;

using binstarjs03.AerialOBJ.Core;
using binstarjs03.AerialOBJ.Core.MinecraftWorld;
using binstarjs03.AerialOBJ.Core.Primitives;
using binstarjs03.AerialOBJ.WpfAppNew2.Components;

namespace binstarjs03.AerialOBJ.WpfAppNew2.Services;
public class SingleThreadedChunkRegionManagerService : IChunkRegionManagerService
{
    private const int s_regionBufferSize = 15;

    private readonly IRegionLoaderService _regionLoaderService;
    private readonly Random _rng = new();

    private readonly StructLock<Point2ZRange<int>> _visibleRegionRange = new();
    private readonly StructLock<Point2ZRange<int>> _visibleChunkRange = new();

    private readonly Dictionary<Point2Z<int>, Region> _loadedRegions = new(s_regionBufferSize);
    private readonly Dictionary<Point2Z<int>, Region> _cachedRegions = new(s_regionBufferSize);
    private readonly List<Point2Z<int>> _pendingRegions = new(s_regionBufferSize);
    private readonly StructLock<Point2Z<int>?> _workedRegion = new() { Value = null };

    public SingleThreadedChunkRegionManagerService(IRegionLoaderService regionLoaderService)
    {
        _regionLoaderService = regionLoaderService;
    }

    public int CachedRegionsCount => _cachedRegions.Count;
    public Point2ZRange<int> VisibleRegionRange => _visibleRegionRange.Value;
    public int LoadedRegionsCount => _loadedRegions.Count;
    public int PendingRegionsCount => _pendingRegions.Count;
    public Point2Z<int>? WorkedRegion => _workedRegion.Value;
    public Point2ZRange<int> VisibleChunkRange => _visibleChunkRange.Value;

    public event Action<Region>? RegionLoaded;
    public event Action<Region>? RegionUnloaded;
    public event RegionReadingErrorHandler? RegionReadingError;
    public event Action<string>? PropertyChanged;

    public void Update(Point2Z<float> cameraPos, float unitMultiplier, Size<int> screenSize)
    {
        if (RecalculateVisibleChunkRange(cameraPos, unitMultiplier, screenSize))
            if (RecalculateVisibleRegionRange())
                ManageRegions();
    }

    public void Reinitialize()
    {
        _pendingRegions.Clear();
        foreach (Region region in _loadedRegions.Values)
            UnloadRegion(region);
        _cachedRegions.Clear();
        _visibleRegionRange.Value = new Point2ZRange<int>(0, 0, 0, 0);
        _workedRegion.Value = null;

        _visibleChunkRange.Value = new Point2ZRange<int>(0, 0, 0, 0);

        OnPropertyChanged(nameof(PendingRegionsCount));
        OnPropertyChanged(nameof(LoadedRegionsCount));
        OnPropertyChanged(nameof(CachedRegionsCount));
        OnPropertyChanged(nameof(VisibleRegionRange));
        OnPropertyChanged(nameof(WorkedRegion));

        OnPropertyChanged(nameof(VisibleChunkRange));
    }

    private bool RecalculateVisibleChunkRange(Point2Z<float> cameraPos, float unitMultiplier, Size<int> screenSize)
    {
        float pixelPerChunk = unitMultiplier * Section.BlockCount; // one unit (or pixel) equal to one block
        Rangeof<int> visibleChunkXRange = calculateVisibleChunkRange(screenSize.Width, pixelPerChunk, cameraPos.X);
        Rangeof<int> visibleChunkZRange = calculateVisibleChunkRange(screenSize.Height, pixelPerChunk, cameraPos.Z);
        Point2ZRange<int> oldVisibleChunkRange = _visibleChunkRange.Value;
        Point2ZRange<int> newVisibleChunkRange = new(visibleChunkXRange, visibleChunkZRange);
        if (newVisibleChunkRange == oldVisibleChunkRange)
            return false;
        _visibleChunkRange.Value = newVisibleChunkRange;
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
        Rangeof<int> visibleRegionXRange = calculateVisibleRegionRange(_visibleChunkRange.Value.XRange);
        Rangeof<int> visibleRegionZRange = calculateVisibleRegionRange(_visibleChunkRange.Value.ZRange);
        Point2ZRange<int> oldVisibleRegionRange = _visibleRegionRange.Value;
        Point2ZRange<int> newVisibleRegionRange = new(visibleRegionXRange, visibleRegionZRange);
        if (newVisibleRegionRange == oldVisibleRegionRange)
            return false;
        _visibleRegionRange.Value = newVisibleRegionRange;
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
            LoadRegions();
    }

    private void UnloadCulledRegions()
    {
        // perform boundary range checking for regions outside display frame
        // unload them if outside
        foreach ((Point2Z<int> regionCoords, Region region) in _loadedRegions)
            if (_visibleRegionRange.Value.IsOutside(regionCoords))
                UnloadRegion(region);

        // remove all pending region list that is no longer visible
        _pendingRegions.RemoveAll(_visibleRegionRange.Value.IsOutside);
        OnPropertyChanged(nameof(PendingRegionsCount));
    }

    private bool QueuePendingRegions()
    {
        bool hasPendingRegion = false;
        for (int x = _visibleRegionRange.Value.XRange.Min; x <= _visibleRegionRange.Value.XRange.Max; x++)
            for (int z = _visibleRegionRange.Value.ZRange.Min; z <= _visibleRegionRange.Value.ZRange.Max; z++)
            {
                Point2Z<int> regionCoords = new(x, z);
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
        OnPropertyChanged(nameof(PendingRegionsCount));
        return hasPendingRegion;
    }

    private void LoadRegions()
    {
        // we cannot do "while (_pendingRegionList.Count > 0)"
        // because we need to lock _pendingRegionList
        while (_pendingRegions.Count > 0)
        {
            Point2Z<int> regionCoords = getRandomPendingRegion();
            _workedRegion.Value = regionCoords;
            OnPropertyChanged(nameof(WorkedRegion));

            Region region;
            if (_cachedRegions.ContainsKey(regionCoords))
                region = _cachedRegions[regionCoords];
            else
            {
                Region? loadResult = _regionLoaderService.LoadRegion(regionCoords, out Exception? e);
                if (loadResult is null)
                {
                    if (e is not null)
                        RegionReadingError?.Invoke(regionCoords, e);
                    continue;
                }
                region = loadResult;
                _cachedRegions.Add(regionCoords, region);
                OnPropertyChanged(nameof(CachedRegionsCount));
            }
            LoadRegion(region);
        }
        _workedRegion.Value = null;
        OnPropertyChanged(nameof(WorkedRegion));

        Point2Z<int> getRandomPendingRegion()
        {
            int randomIndex = _rng.Next(0, _pendingRegions.Count);
            Point2Z<int> result = _pendingRegions[randomIndex];
            _pendingRegions.RemoveAt(randomIndex);
            OnPropertyChanged(nameof(PendingRegionsCount));
            return result;
        }
    }

    private void LoadRegion(Region region)
    {
        if (_visibleRegionRange.Value.IsOutside(region.RegionCoords)
            || _loadedRegions.ContainsKey(region.RegionCoords))
            return;
        _loadedRegions.Add(region.RegionCoords, region);
        OnPropertyChanged(nameof(LoadedRegionsCount));
        RegionLoaded?.Invoke(region);
    }

    private void UnloadRegion(Region region)
    {
        _loadedRegions.Remove(region.RegionCoords);
        OnPropertyChanged(nameof(LoadedRegionsCount));
        RegionUnloaded?.Invoke(region);
    }

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(propertyName);
    }
}
