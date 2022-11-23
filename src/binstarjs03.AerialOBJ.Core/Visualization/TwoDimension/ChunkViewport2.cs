using System;
using System.Collections.Generic;

using binstarjs03.AerialOBJ.Core.MinecraftWorld;
using binstarjs03.AerialOBJ.Core.Primitives;

namespace binstarjs03.AerialOBJ.Core.Visualization.TwoDimension;
//#pragma warning disable
public abstract class ChunkViewport2<TRegionImage> : Viewport2 where TRegionImage : class, IRegionImage, new()
{
    public event RegionImageEventHandler? RegionImageAdded;
    public event RegionImageEventHandler? RegionImageRemoved;
    public delegate void RegionImageEventHandler(TRegionImage regionImage);

    private const int s_regionBufferSize = 20;
    private const int s_chunkBufferSize = s_regionBufferSize * Region.TotalChunkCount;

    private readonly Random _rng = new();

    private Point2ZRange<int> _visibleRegionRange;
    private readonly Dictionary<Point2Z<int>, RegionModel<TRegionImage>> _loadedRegions = new(s_regionBufferSize);
    private readonly List<Point2Z<int>> _pendingRegionList = new(s_regionBufferSize);
    private Point2Z<int>? _workedRegion = null;

    private Point2ZRange<int> _visibleChunkRange;

    public Point2<int> ScreenCenter => (Point2<int>)(ScreenSize / 2);
    public float PixelPerBlock => ZoomLevel;
    public float PixelPerChunk => PixelPerBlock * Section.BlockCount;
    public float PixelPerRegion => PixelPerChunk * Region.ChunkCount;

    public ChunkViewport2(Size<int> screenSize) : base(screenSize)
    {

    }

    protected override void OnUpdate()
    {
        if (RecalculateVisibleChunkRange())
        {
            if (RecalculateVisibleRegionRange())
                LoadUnloadRegions();
        }
    }

    private bool RecalculateVisibleChunkRange()
    {
        // Camera chunk in here means which chunk the camera is pointing to.
        // Here we dont use int because floating point accuracy is crucial
        double xCameraChunk = CameraPos.X / Section.BlockCount;
        double zCameraChunk = CameraPos.Z / Section.BlockCount;

        // min/maxCanvasCenterChunk means which chunk that is visible at the edgemost of the control
        // that is measured by the length or height of the control size (which is chunk canvas)
        double minXCanvasCenterChunk = -(ScreenCenter.X / PixelPerChunk);
        double maxXCanvasCenterChunk = ScreenCenter.X / PixelPerChunk;
        int minX = MathUtils.Floor(xCameraChunk + minXCanvasCenterChunk);
        int maxX = MathUtils.Floor(xCameraChunk + maxXCanvasCenterChunk);
        Rangeof<int> visibleChunkXRange = new(minX, maxX);

        double minZCanvasCenterChunk = -(ScreenCenter.Y / PixelPerChunk);
        double maxZCanvasCenterChunk = ScreenCenter.Y / PixelPerChunk;
        int minZ = MathUtils.Floor(zCameraChunk + minZCanvasCenterChunk);
        int maxZ = MathUtils.Floor(zCameraChunk + maxZCanvasCenterChunk);
        Rangeof<int> visibleChunkZRange = new(minZ, maxZ);

        Point2ZRange<int> oldVisibleChunkRange = _visibleChunkRange;
        Point2ZRange<int> newVisibleChunkRange = new(visibleChunkXRange, visibleChunkZRange);
        if (newVisibleChunkRange == oldVisibleChunkRange)
            return false;
        _visibleChunkRange = newVisibleChunkRange;
        return true;
    }

    private bool RecalculateVisibleRegionRange()
    {
        Point2ZRange<int> vcr = _visibleChunkRange;

        // Calculating region range is easier since we only need to
        // divide the range by how many chunks in region (in single axis)

        int regionMinX = MathUtils.DivFloor(vcr.XRange.Min, Region.ChunkCount);
        int regionMaxX = MathUtils.DivFloor(vcr.XRange.Max, Region.ChunkCount);
        Rangeof<int> visibleRegionXRange = new(regionMinX, regionMaxX);

        int regionMinZ = MathUtils.DivFloor(vcr.ZRange.Min, Region.ChunkCount);
        int regionMaxZ = MathUtils.DivFloor(vcr.ZRange.Max, Region.ChunkCount);
        Rangeof<int> visibleRegionZRange = new(regionMinZ, regionMaxZ);

        Point2ZRange<int> oldVisibleRegionRange = _visibleRegionRange;
        Point2ZRange<int> newVisibleRegionRange = new(visibleRegionXRange, visibleRegionZRange);
        if (newVisibleRegionRange == oldVisibleRegionRange)
            return false;
        _visibleRegionRange = newVisibleRegionRange;
        return true;
    }

    protected virtual void LoadUnloadRegions()
    {
        // perform boundary range checking for regions outside display frame
        // unload them if outside
        foreach ((Point2Z<int> regionCoords, RegionModel<TRegionImage> regionModel) in _loadedRegions)
            if (_visibleRegionRange.IsOutside(regionCoords))
                UnloadRegionModel(regionModel);

        // remove all pending region that is no longer visible
        _pendingRegionList.RemoveAll(_visibleRegionRange.IsOutside);

        // perform sweep checking from min to max range for visible regions
        // add them to pending list if not loaded yet, not in the list, and is not worked yet
        for (int x = _visibleRegionRange.XRange.Min; x <= _visibleRegionRange.XRange.Max; x++)
            for (int z = _visibleRegionRange.ZRange.Min; z <= _visibleRegionRange.ZRange.Max; z++)
            {
                Point2Z<int> regionCoords = new(x, z);
                if (!_pendingRegionList.Contains(regionCoords))
                    _pendingRegionList.Add(regionCoords);
            }
        // base implementation goes here, may call LoadRegion(), must call base implementation()
    }

    protected void LoadRegion()
    {
        if (_pendingRegionList.Count == 0)
            return;
        int randomIndex = _rng.Next(0, _pendingRegionList.Count);
        Point2Z<int> regionCoords = _pendingRegionList[randomIndex];
        _pendingRegionList.RemoveAt(randomIndex);
        _workedRegion = regionCoords;

        Region? region = GetRegionFromCache(regionCoords);
        if (region is null)
            return;
        RegionModel<TRegionImage> regionModel = new(region);
        _loadedRegions.Add(regionCoords, regionModel);
        RegionImageAdded?.Invoke(regionModel.RegionImage);
    }

    protected abstract Region? GetRegionFromCache(Point2Z<int> regionCoords);
    /* Implementation of GetRegionFromCache for current implementation of WPF App
     *
     * Region region;
     * // Try to fetch region from cache. If miss, load it from disk
     * if (RegionCacheService.HasRegion(regionCoords))
     *     region = RegionCacheService.Get(regionCoords);
     * else
     * {
     *     Region? regionDisk = IOService.ReadRegionFile(regionCoords, out Exception? e);
     *     // cancel region loading if we can't get the region at specified coords
     *     // (corrupted, file not exist or not generated yet etc)
     *     if (regionDisk is null)
     *     {
     *         if (e is not null)
     *         {
     *             // TODO display messagebox only once and never again
     *             LogService.LogEmphasis($"Region {regionCoords} was skipped.", LogService.Emphasis.Error);
     *             LogService.Log($"Cause of exception: {e.GetType()}");
     *             LogService.Log($"Exception details: {e}", useSeparator: true);
     *         }
     *         return;
     *     }
     *     region = regionDisk;
     *     RegionCacheService.Store(region);
     * }
     */

    private void UnloadRegionModel(RegionModel<TRegionImage> regionModel)
    {
        _loadedRegions.Remove(regionModel.RegionCoords);
        RegionImageRemoved?.Invoke(regionModel.RegionImage);
    }
}
