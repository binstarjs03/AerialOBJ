using System.Collections.Generic;
using System.Runtime.CompilerServices;

using binstarjs03.AerialOBJ.Core.MinecraftWorld;
using binstarjs03.AerialOBJ.Core.Primitives;

namespace binstarjs03.AerialOBJ.Core.Visualization.TwoDimension;

public abstract class ChunkViewport2<TRegionImage> : Viewport2 where TRegionImage : class, IRegionImage, new()
{
    public event RegionImageEventHandler? RegionImageAdded;
    public event RegionImageEventHandler? RegionImageRemoved;
    public delegate void RegionImageEventHandler(TRegionImage regionImage);

    private const int s_regionBufferSize = 20;
    private const int s_chunkBufferSize = s_regionBufferSize * Region.TotalChunkCount;

    private Point2ZRange<int> _visibleRegionRange;
    protected readonly Dictionary<Point2Z<int>, RegionModel<TRegionImage>> _loadedRegions = new(s_regionBufferSize);
    protected readonly List<Point2Z<int>> _pendingRegionList = new(s_regionBufferSize);

    private Point2ZRange<int> _visibleChunkRange;
    protected readonly Dictionary<Point2Z<int>, ChunkModel> _loadedChunks = new(s_chunkBufferSize);
    protected readonly List<Point2Z<int>> _pendingChunkList = new(s_chunkBufferSize);
    protected readonly HashSet<Point2Z<int>> _pendingChunkSet = new(s_chunkBufferSize);

    // Public readonly accessors for internal use and for UI display.
    // For properties that has its underlying fields, use field instead for consistency
    public Point2<int> ScreenCenter => (Point2<int>)(ScreenSize / 2);
    public float PixelPerBlock => ZoomLevel;
    public float PixelPerChunk => PixelPerBlock * Section.BlockCount;
    public float PixelPerRegion => PixelPerChunk * Region.ChunkCount;

    public Point2ZRange<int> VisibleRegionRange => _visibleRegionRange;
    public int LoadedRegionCount => _loadedRegions.Count;
    public int PendingRegionCount => _pendingRegionList.Count;

    public Point2ZRange<int> VisibleChunkRange => _visibleChunkRange;
    public int LoadedChunkCount => _loadedChunks.Count;
    public int PendingChunkCount => _pendingChunkList.Count;

    public ChunkViewport2(Size<int> screenSize) : base(screenSize)
    {

    }

    protected override void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        base.OnPropertyChanged(propertyName);
        if (propertyName == nameof(ScreenSize))
        {
            base.OnPropertyChanged(nameof(ScreenCenter));
            base.OnPropertyChanged(nameof(PixelPerBlock));
            base.OnPropertyChanged(nameof(PixelPerChunk));
            base.OnPropertyChanged(nameof(PixelPerRegion));
        }
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
        OnPropertyChanged(nameof(VisibleChunkRange));
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
        OnPropertyChanged(nameof(VisibleRegionRange));
        return true;
    }

    protected virtual void LoadUnloadRegions()
    {
        // perform boundary range checking for regions outside display frame
        // unload them if outside
        foreach ((Point2Z<int> regionCoords, RegionModel<TRegionImage> regionModel) in _loadedRegions)
            if (_visibleRegionRange.IsOutside(regionCoords))
                UnloadRegionModel(regionModel);
        OnPropertyChanged(nameof(LoadedRegionCount));

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
        OnPropertyChanged(nameof(PendingRegionCount));
        // base implementation goes here, may call LoadRegion(), must call base implementation()
    }

    protected void LoadRegionModel(RegionModel<TRegionImage> regionModel)
    {
        if (_visibleRegionRange.IsOutside(regionModel.RegionCoords)
            || _loadedRegions.ContainsKey(regionModel.RegionCoords))
            return;
        _loadedRegions.Add(regionModel.RegionCoords, regionModel);
        RegionImageAdded?.Invoke(regionModel.RegionImage);
    }

    protected void UnloadRegionModel(RegionModel<TRegionImage> regionModel)
    {
        _loadedRegions.Remove(regionModel.RegionCoords);
        RegionImageRemoved?.Invoke(regionModel.RegionImage);
    }

    protected virtual void LoadUnloadChunks()
    {
        // remove all pending chunk that is no longer visible
        _pendingChunkList.RemoveAll(_visibleChunkRange.IsOutside);
        _pendingChunkSet.RemoveWhere(_visibleChunkRange.IsOutside);

        // perform sweep-checking from min range to max range for chunks inside display frame
        for (int x = _visibleChunkRange.XRange.Min; x <= _visibleChunkRange.XRange.Max; x++)
            for (int z = _visibleChunkRange.ZRange.Min; z <= _visibleChunkRange.ZRange.Max; z++)
            {
                Point2Z<int> chunkCoordsAbs = new(x, z);
                // set is a whole lot faster to check for item existence
                // if the content has hundreds of items, especially for
                // tight-loop like this (approx. millions of comparison performed)
                if (_pendingChunkSet.Contains(chunkCoordsAbs))
                    continue;
                _pendingChunkSet.Add(chunkCoordsAbs);
                _pendingChunkList.Add(chunkCoordsAbs);
            }
        OnPropertyChanged(nameof(PendingChunkCount));
    }
}
