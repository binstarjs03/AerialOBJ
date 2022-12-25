using System;
using System.ComponentModel;

using binstarjs03.AerialOBJ.Core;
using binstarjs03.AerialOBJ.Core.MinecraftWorld;
using binstarjs03.AerialOBJ.Core.Primitives;
using binstarjs03.AerialOBJ.WpfAppNew2.Models;

using CommunityToolkit.Mvvm.ComponentModel;

namespace binstarjs03.AerialOBJ.WpfAppNew2.Services;
[ObservableObject]
public partial class ConcurrentChunkRegionManagerService : IChunkRegionManagerService
{
    [ObservableProperty] private Point2ZRange<int> _visibleChunkRange;
    [ObservableProperty] private Point2ZRange<int> _visibleRegionRange;

    public event Action<RegionImageModel>? RegionImageAdded;
    public event Action<RegionImageModel>? RegionImageRemoved;
    public event RegionReadingErrorHandler? RegionReadingError;
    public event Action<string>? PropertyChanged2;

    public ConcurrentChunkRegionManagerService()
    {
        PropertyChanged += (sender, e) =>
        {
            if (e.PropertyName is not null)
                PropertyChanged2?.Invoke(e.PropertyName);
        };
    }

    public void Update(Point2Z<float> cameraPos, float unitMultiplier, Size<int> screenSize)
    {
        if (RecalculateVisibleChunkRange(cameraPos, unitMultiplier, screenSize))
            RecalculateVisibleRegionRange();
    }

    private bool RecalculateVisibleChunkRange(Point2Z<float> cameraPos, float unitMultiplier, Size<int> screenSize)
    {
        Point2Z<int> worldScreenCenter = (Point2Z<int>)(screenSize / 2);
        float pixelPerChunk = unitMultiplier * Section.BlockCount; // one unit (or pixel) equal to one block

        // Camera chunk in here means which chunk the camera is pointing to.
        // Here we dont use int because floating point accuracy is crucial
        double xCameraChunk = cameraPos.X / Section.BlockCount;
        double zCameraChunk = cameraPos.Z / Section.BlockCount;

        // min/maxCanvasCenterChunk means which chunk that is visible at the edgemost of the control
        // that is measured by the length or height of the control size (which is chunk canvas)
        double minXCanvasCenterChunk = -(worldScreenCenter.X / pixelPerChunk);
        double maxXCanvasCenterChunk = worldScreenCenter.X / pixelPerChunk;
        int minX = MathUtils.Floor(xCameraChunk + minXCanvasCenterChunk);
        int maxX = MathUtils.Floor(xCameraChunk + maxXCanvasCenterChunk);
        Rangeof<int> visibleChunkXRange = new(minX, maxX);

        double minZCanvasCenterChunk = -(worldScreenCenter.Z / pixelPerChunk);
        double maxZCanvasCenterChunk = worldScreenCenter.Z / pixelPerChunk;
        int minZ = MathUtils.Floor(zCameraChunk + minZCanvasCenterChunk);
        int maxZ = MathUtils.Floor(zCameraChunk + maxZCanvasCenterChunk);
        Rangeof<int> visibleChunkZRange = new(minZ, maxZ);

        Point2ZRange<int> oldVisibleChunkRange = VisibleChunkRange;
        Point2ZRange<int> newVisibleChunkRange = new(visibleChunkXRange, visibleChunkZRange);
        if (newVisibleChunkRange == oldVisibleChunkRange)
            return false;
        VisibleChunkRange = newVisibleChunkRange;
        return true;
    }

    private bool RecalculateVisibleRegionRange()
    {
        // Calculating region range is easier since we only need to
        // divide the range by how many chunks in region (in single axis)

        int regionMinX = MathUtils.DivFloor(VisibleChunkRange.XRange.Min, Region.ChunkCount);
        int regionMaxX = MathUtils.DivFloor(VisibleChunkRange.XRange.Max, Region.ChunkCount);
        Rangeof<int> visibleRegionXRange = new(regionMinX, regionMaxX);

        int regionMinZ = MathUtils.DivFloor(VisibleChunkRange.ZRange.Min, Region.ChunkCount);
        int regionMaxZ = MathUtils.DivFloor(VisibleChunkRange.ZRange.Max, Region.ChunkCount);
        Rangeof<int> visibleRegionZRange = new(regionMinZ, regionMaxZ);

        Point2ZRange<int> oldVisibleRegionRange = VisibleRegionRange;
        Point2ZRange<int> newVisibleRegionRange = new(visibleRegionXRange, visibleRegionZRange);
        if (newVisibleRegionRange == oldVisibleRegionRange)
            return false;
        VisibleRegionRange = newVisibleRegionRange;
        return true;
    }
}
