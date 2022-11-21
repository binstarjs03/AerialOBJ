using System;
using System.Collections.Generic;
using System.Windows;

using binstarjs03.AerialOBJ.Core;
using binstarjs03.AerialOBJ.Core.CoordinateSystem;
using binstarjs03.AerialOBJ.Core.MinecraftWorld;
using binstarjs03.AerialOBJ.WpfAppNew.Models;

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

    private int _heightLimit = 319;
    private CoordsRange2 _visibleRegionRange = new();
    private CoordsRange2 _visibleChunkRange = new();
    private List<RegionModel> _regionModels = new();

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
        get => _visibleChunkRange;
        set
        {
            if (value != _visibleChunkRange)
            {
                _visibleChunkRange = value;
                VisibleChunkRangeChanged?.Invoke();
            }
        }
    }
    #endregion Properties

    public ChunkRegionViewport() : base()
    {
        ScreenSizeChanged += OnScreenCenterChanged;
        ZoomLevelChanged += OnZoomLevelChanged;
    }

    private void OnZoomLevelChanged()
    {
        PixelPerBlockChanged?.Invoke();
        PixelPerChunkChanged?.Invoke();
        PixelPerRegionChanged?.Invoke();
    }

    private void OnScreenCenterChanged() => ScreenCenterChanged?.Invoke();

    protected override void OnUpdate()
    {
        if (RecalculateVisibleChunkRange())
        {
            if (RecalculateVisibleRegionRange())
            {

            }
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
}
