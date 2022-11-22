using System;
using System.Collections.Generic;
using System.Windows;

using binstarjs03.AerialOBJ.Core;
using binstarjs03.AerialOBJ.Core.CoordinateSystem;
using binstarjs03.AerialOBJ.Core.MinecraftWorld;

using Range = binstarjs03.AerialOBJ.Core.Range;
using Region = binstarjs03.AerialOBJ.Core.MinecraftWorld.Region;

namespace binstarjs03.AerialOBJ.WpfAppNew.Components;
public class ChunkRegionViewport : Viewport
{
    public event Action? ScreenCenterChanged;
    public event Action? PixelPerBlockChanged;
    public event Action? PixelPerChunkChanged;
    public event Action? PixelPerRegionChanged;
    public event Action? HeightLimitChanged;
    public event Action? VisibleRegionRangeChanged;
    public event Action? VisibleChunkRangeChanged;
    public event Action? PendingRegionCountChanged;
    public event Action? PendingChunkCountChanged;

    private const int s_regionBufferSize = 15;
    private const int s_chunkBufferSize = s_regionBufferSize * Region.TotalChunkCount;

    private readonly List<Coords2> _pendingRegionList = new(s_regionBufferSize);

    private readonly List<Coords2> _pendingChunkList = new(s_chunkBufferSize);
    private readonly HashSet<Coords2> _pendingChunkSet = new(s_chunkBufferSize);

    private int _heightLimit = 319;
    private CoordsRange2 _visibleRegionRange = new();
    private CoordsRange2 _visibleChunkRange = new();

    #region Properties
    public Point ScreenCenter => new(ScreenSize.Width / 2, ScreenSize.Height / 2);
    public double PixelPerBlock => ZoomLevel;
    public double PixelPerChunk => PixelPerBlock * Section.BlockCount;
    public double PixelPerRegion => PixelPerChunk * Region.ChunkCount;
    public int HeightLimit
    {
        get => _heightLimit;
        set
        {
            if (value != _heightLimit)
            {
                _heightLimit = value;
                HeightLimitChanged?.Invoke();
            }
        }
    }
    public CoordsRange2 VisibleRegionRange
    {
        get => _visibleRegionRange;
        set
        {
            if (value != _visibleRegionRange)
            {
                _visibleRegionRange = value;
                VisibleRegionRangeChanged?.Invoke();
            }
        }
    }
    public CoordsRange2 VisibleChunkRange
    {
        get=>_visibleChunkRange;
        set
        {
            if (value != _visibleChunkRange)
            {
                _visibleChunkRange = value;
                VisibleChunkRangeChanged?.Invoke();
            }
        }
    }

    public int PendingRegionCount => _pendingRegionList.Count;
    public int PendingChunkCount => _pendingChunkList.Count;
    #endregion Properties

    public ChunkRegionViewport() : base()
    {
        ScreenSizeChanged += OnScreenSizeChanged;
        ZoomLevelChanged += OnZoomLevelChanged;
    }

    private void OnZoomLevelChanged()
    {
        PixelPerBlockChanged?.Invoke();
        PixelPerChunkChanged?.Invoke();
        PixelPerRegionChanged?.Invoke();
    }

    private void OnScreenSizeChanged() => ScreenCenterChanged?.Invoke();

    protected override void OnUpdate()
    {
        if (RecalculateVisibleChunkRange())
        {
            if (RecalculateVisibleRegionRange())
                LoadUnloadRegions();
            LoadUnloadChunks();
        }
    }

    private bool RecalculateVisibleChunkRange()
    {
        // Camera chunk in here means which chunk the camera is pointing to.
        // Here we dont use int because floating point accuracy is crucial
        double xCameraChunk = CameraPos.X / Section.BlockCount;
        double zCameraChunk = CameraPos.Y / Section.BlockCount;

        // min/maxCanvasCenterChunk means which chunk that is visible at the edgemost of the control
        // that is measured by the length or height of the control size (which is chunk canvas)
        double minXCanvasCenterChunk = -(ScreenCenter.X / PixelPerChunk);
        double maxXCanvasCenterChunk = ScreenCenter.X / PixelPerChunk;
        int minX = MathUtils.Floor(xCameraChunk + minXCanvasCenterChunk);
        int maxX = MathUtils.Floor(xCameraChunk + maxXCanvasCenterChunk);
        Range visibleChunkXRange = new(minX, maxX);

        double minZCanvasCenterChunk = -(ScreenCenter.Y / PixelPerChunk);
        double maxZCanvasCenterChunk = ScreenCenter.Y / PixelPerChunk;
        int minZ = (int)Math.Floor(zCameraChunk + minZCanvasCenterChunk);
        int maxZ = (int)Math.Floor(zCameraChunk + maxZCanvasCenterChunk);
        Range visibleChunkZRange = new(minZ, maxZ);

        CoordsRange2 oldVisibleChunkRange = VisibleChunkRange;
        CoordsRange2 newVisibleChunkRange = new(visibleChunkXRange, visibleChunkZRange);
        if (newVisibleChunkRange == oldVisibleChunkRange)
            return false;
        VisibleChunkRange = newVisibleChunkRange;
        return true;
    }

    private bool RecalculateVisibleRegionRange()
    {
        CoordsRange2 vcr = VisibleChunkRange;

        // Calculating region range is easier since we only need to
        // divide the range by how many chunks in region (in single axis)

        int regionMinX = MathUtils.DivFloor(vcr.XRange.Min, Region.ChunkCount);
        int regionMaxX = MathUtils.DivFloor(vcr.XRange.Max, Region.ChunkCount);
        Range visibleRegionXRange = new(regionMinX, regionMaxX);

        int regionMinZ = MathUtils.DivFloor(vcr.ZRange.Min, Region.ChunkCount);
        int regionMaxZ = MathUtils.DivFloor(vcr.ZRange.Max, Region.ChunkCount);
        Range visibleRegionZRange = new(regionMinZ, regionMaxZ);

        CoordsRange2 oldVisibleRegionRange = VisibleRegionRange;
        CoordsRange2 newVisibleRegionRange = new(visibleRegionXRange, visibleRegionZRange);
        if (newVisibleRegionRange == oldVisibleRegionRange)
            return false;
        VisibleRegionRange = newVisibleRegionRange;
        return true;
    }

    private void LoadUnloadRegions()
    {
        // remove all pending region that is no longer visible
        _pendingRegionList.RemoveAll(regionCoords => VisibleRegionRange.IsOutside(regionCoords));

        // perform sweep checking from min to max range for visible regions
        // add them to pending list if not loaded yet, not in the list, and is not worked yet
        for (int x = VisibleRegionRange.XRange.Min; x <= VisibleRegionRange.XRange.Max; x++)
            for (int z = VisibleRegionRange.ZRange.Min; z <= VisibleRegionRange.ZRange.Max; z++)
            {
                Coords2 regionCoords = new(x, z);
                if (!_pendingRegionList.Contains(regionCoords))
                    _pendingRegionList.Add(regionCoords);
            }
        PendingRegionCountChanged?.Invoke();
    }

    private void LoadUnloadChunks()
    {
        // remove all pending chunk that is no longer visible
        _pendingChunkList.RemoveAll(chunkCoordsAbs => !_visibleChunkRange.IsInside(chunkCoordsAbs));
        _pendingChunkSet.RemoveWhere(chunkCoordsAbs => !_visibleChunkRange.IsInside(chunkCoordsAbs));

        // perform sweep-checking from min range to max range for chunks inside display frame
        for (int x = _visibleChunkRange.XRange.Min; x <= _visibleChunkRange.XRange.Max; x++)
            for (int z = _visibleChunkRange.ZRange.Min; z <= _visibleChunkRange.ZRange.Max; z++)
            {
                Coords2 chunkCoordsAbs = new(x, z);
                // set is a whole lot faster to check for item existence
                // if the content has hundreds of items, especially for
                // tight-loop like this (approx. millions of comparison performed)
                if (_pendingChunkSet.Contains(chunkCoordsAbs))
                    continue;
                _pendingChunkSet.Add(chunkCoordsAbs);
                _pendingChunkList.Add(chunkCoordsAbs);
            }
        PendingChunkCountChanged?.Invoke();
    }
}
