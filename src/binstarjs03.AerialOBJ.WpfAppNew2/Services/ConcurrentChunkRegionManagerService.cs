using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using binstarjs03.AerialOBJ.Core;
using binstarjs03.AerialOBJ.Core.MinecraftWorld;
using binstarjs03.AerialOBJ.Core.Primitives;
using binstarjs03.AerialOBJ.WpfAppNew2.Models;

namespace binstarjs03.AerialOBJ.WpfAppNew2.Services;
public class ConcurrentChunkRegionManagerService : IChunkRegionManagerService
{
    private const int s_regionBufferSize = 15;
    private const int s_chunkBufferSize = s_regionBufferSize * Region.TotalChunkCount;

    private Point2ZRange<int> _visibleRegionRange;
    private readonly Dictionary<Point2Z<int>, RegionDataImagePair> _loadedRegions = new(s_regionBufferSize);
    private readonly List<Point2Z<int>> _pendingRegionList = new(s_regionBufferSize);
    private Point2Z<int>? _workedRegion = null;
    private Task _workRegionTask = new(() => { });

    private Point2ZRange<int> _visibleChunkRange;

    public Point2ZRange<int> VisibleRegionRange => _visibleRegionRange;
    public int LoadedRegionsCount => _loadedRegions.Count;
    public int PendingRegionsCount => _pendingRegionList.Count;
    public Point2Z<int>? WorkedRegion => _workedRegion;
    public bool NoWorkedRegion => _workedRegion is null;
    public Point2ZRange<int> VisibleChunkRange => _visibleChunkRange;

    public event Action<RegionImageModel>? RegionImageAdded;
    public event Action<RegionImageModel>? RegionImageRemoved;
    public event RegionReadingErrorHandler? RegionReadingError;
    public event Action<string>? PropertyChanged;

    public void Update(Point2Z<float> cameraPos, float unitMultiplier, Size<int> screenSize)
    {
        if (RecalculateVisibleChunkRange(cameraPos, unitMultiplier, screenSize))
            if (RecalculateVisibleRegionRange())
                ManageRegions();
    }

    private bool RecalculateVisibleChunkRange(Point2Z<float> cameraPos, float unitMultiplier, Size<int> screenSize)
    {
        float pixelPerChunk = unitMultiplier * Section.BlockCount; // one unit (or pixel) equal to one block

        Rangeof<int> visibleChunkXRange = calculateVisibleChunkRange(screenSize.Width, pixelPerChunk, cameraPos.X);
        Rangeof<int> visibleChunkZRange = calculateVisibleChunkRange(screenSize.Height, pixelPerChunk, cameraPos.Z);

        Point2ZRange<int> oldVisibleChunkRange = _visibleChunkRange;
        Point2ZRange<int> newVisibleChunkRange = new(visibleChunkXRange, visibleChunkZRange);
        if (newVisibleChunkRange == oldVisibleChunkRange)
            return false;
        _visibleChunkRange = newVisibleChunkRange;
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
        Rangeof<int> visibleRegionXRange = calculateVisibleRegionRange(_visibleChunkRange.XRange);
        Rangeof<int> visibleRegionZRange = calculateVisibleRegionRange(_visibleChunkRange.ZRange);

        Point2ZRange<int> oldVisibleRegionRange = _visibleRegionRange;
        Point2ZRange<int> newVisibleRegionRange = new(visibleRegionXRange, visibleRegionZRange);
        if (newVisibleRegionRange == oldVisibleRegionRange)
            return false;
        _visibleRegionRange = newVisibleRegionRange;
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
        LoadRegions();
    }

    private void UnloadCulledRegions()
    {
        // perform boundary range checking for regions outside display frame
        // unload them if outside
        foreach ((Point2Z<int> regionCoords, RegionDataImagePair region)in _loadedRegions)
            if (_visibleRegionRange.IsOutside(regionCoords))
                UnloadRegion(region);
        OnPropertyChanged(nameof(LoadedRegionsCount));

        // remove all pending region list that is no longer visible
        _pendingRegionList.RemoveAll(_visibleRegionRange.IsOutside);
        OnPropertyChanged(nameof(PendingRegionsCount));
    }

    private void LoadRegions()
    {
        for (int x = _visibleRegionRange.XRange.Min; x <= _visibleRegionRange.XRange.Max; x++)
            for (int z = _visibleRegionRange.ZRange.Min; z <= _visibleRegionRange.ZRange.Max; z++)
            {
                Point2Z<int> regionCoords = new(x, z);
                if (_loadedRegions.ContainsKey(regionCoords)
                    || _pendingRegionList.Contains(regionCoords)
                    || _workedRegion == regionCoords)
                    continue;
                else
                    _pendingRegionList.Add(regionCoords);
            }
        OnPropertyChanged(nameof(PendingRegionsCount));
        if (_workRegionTask.Status != TaskStatus.Running)
        {

        }
    }


    private void UnloadRegion(RegionDataImagePair region) { }

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(propertyName);
    }
}
