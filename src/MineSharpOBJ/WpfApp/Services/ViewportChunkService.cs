using System;
using System.Collections.Generic;

using binstarjs03.MineSharpOBJ.Core.CoordinateSystem;
using binstarjs03.MineSharpOBJ.Core.RegionMc;
using binstarjs03.MineSharpOBJ.WpfApp.UIElements.Controls;

namespace binstarjs03.MineSharpOBJ.WpfApp.Services;

using binstarjs03.MineSharpOBJ.Core;

public class ViewportChunkService
{
    private readonly ViewportControlVM _viewportVM;
    private CoordsRange2 _visibleChunkRange;
    private List<Coords2> _visibleChunksCoords = new();
    private readonly List<ChunkImageControl> _visibleChunkImageControl = new();

    public ViewportChunkService(ViewportControlVM viewport)
    {
        _viewportVM = viewport;
    }

    public CoordsRange2 VisibleChunkRange
    {
        get => _visibleChunkRange;
        private set => _visibleChunkRange = value;
    }

    public void Update()
    {
        UpdateVisibleChunkRange();
    }

    public void UpdateVisibleChunkRange()
    {
        // TODO so much doubles here. double calculate slower than integer
        // in arithmetic operation right?
        ViewportControlVM vm = _viewportVM;

        // camera chunk in here means which chunk the camera is pointing to
        double xCameraChunk = vm.ViewportCameraPos.X / Section.BlockCount;
        double yCameraChunk = vm.ViewportCameraPos.Y / Section.BlockCount;

        // min/maxCanvasCenterChunk means which chunk that is visible at the edgemost of the control
        // that is measured by the length/height of the control size (which is chunk canvas)
        double minXCanvasCenterChunk = -(vm.ViewportChunkCanvasCenter.X / vm.ViewportPixelPerChunk);
        double maxXCanvasCenterChunk = vm.ViewportChunkCanvasCenter.X / vm.ViewportPixelPerChunk;

        int minX = (int)Math.Floor(Math.Round(xCameraChunk + minXCanvasCenterChunk, 3));
        int maxX = (int)Math.Floor(Math.Round(xCameraChunk + maxXCanvasCenterChunk, 3));
        Range visibleChunkXRange = new(minX, maxX);

        double minYCanvasCenterChunk = -(vm.ViewportChunkCanvasCenter.Y / vm.ViewportPixelPerChunk);
        double maxYCanvasCenterChunk = vm.ViewportChunkCanvasCenter.Y / vm.ViewportPixelPerChunk;

        int minZ = (int)Math.Floor(Math.Round(yCameraChunk + minYCanvasCenterChunk, 3));
        int maxZ = (int)Math.Floor(Math.Round(yCameraChunk + maxYCanvasCenterChunk, 3));
        Range visibleChunkZRange = new(minZ, maxZ);

        _visibleChunkRange = new CoordsRange2(visibleChunkXRange, visibleChunkZRange);
    }
}
