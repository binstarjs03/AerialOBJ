using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using binstarjs03.MineSharpOBJ.WpfApp.Models;
using binstarjs03.MineSharpOBJ.WpfApp.UIElements.Windows;

using Point = binstarjs03.MineSharpOBJ.Core.Utils.Point;
using PointF = binstarjs03.MineSharpOBJ.Core.Utils.PointF;
using Section = binstarjs03.MineSharpOBJ.Core.RegionMc.Section;

namespace binstarjs03.MineSharpOBJ.WpfApp.UIElements.Controls;

// TODO UX improvement: wrap mouse around viewport like in blender 3D did
// when mouse goes outside the viewport

public partial class ViewportControl : UserControl, IViewModel<ViewportControlViewModel, ViewportControl> {
    private static readonly int[] s_zoomBlockPixelCount = new int[] {
        1, 2, 3, 5, 8, 13, 21, 34
    };

    // Constructor for Design-Time
    // CS8625: Cannot convert null literal to non-nullable reference type.
#   pragma warning disable CS8625
    public ViewportControl() : this(null) { }
#pragma warning restore CS8625

    public ViewportControl(MainWindow mainWindow) {
        MainWindow = mainWindow;
        InitializeComponent();
        ViewModel = new ViewportControlViewModel(this);
        ViewportControlViewModel.Context = ViewModel;
        DataContext = ViewModel;

        ViewportCameraPos = PointF.Origin;
        ViewportZoomLevel = 2;
        MousePos = PointF.Origin;
        MousePosDelta = PointF.Origin;
        MouseIsClickHolding = false;
        MouseInitClickDrag = true;
        MouseIsOutside = true;

        for (int x = -8; x < 16; x++) {
            for (int y = -8; y < 16; y++) {
                ChunkModel chunk = new(new(x, y));
                chunk.SetRandomImage((x + y) % 2 == 0);
                ChunkCanvas.Children.Add(chunk);
                chunk.Width = ViewportPixelPerChunk;
            }
        }
    }

    public MainWindow MainWindow { get; private set; }
    public ViewportControlViewModel ViewModel { get; private set; }

    public PointF ViewportCameraPos { get; private set; }
    public PointF ViewportChunkPosOffset => ViewportCameraPos * ViewportPixelPerBlock;
    public int ViewportZoomLevel { get; private set; }
    public int ViewportPixelPerBlock => s_zoomBlockPixelCount[ViewportZoomLevel - 1];
    public int ViewportPixelPerChunk => ViewportPixelPerBlock * Section.BlockCount;

    public PointF MousePos { get; private set; }
    public PointF MousePosDelta { get; private set; }
    public bool MouseIsClickHolding { get; set; }
    public bool MouseInitClickDrag { get; set; }
    public bool MouseIsOutside { get; set ; }

    public void SetCameraPosition(PointF cameraPos) {
        ViewportCameraPos = cameraPos;
        UpdateChunkTransformation(updateChunkSize: false);
        UpdateDebugPanel();
    }

    private void UpdateChunkTransformation(bool updateChunkSize) {
        PointF chunkCanvasCenter = new(
            ChunkCanvas.ActualWidth / 2,
            ChunkCanvas.ActualHeight / 2
        );
        foreach (ChunkModel chunk in ChunkCanvas.Children) {
            if (updateChunkSize)
                UpdateChunkSize(chunk);
            UpdateChunkPosition(chunk, chunkCanvasCenter);
        }
    }

    private void UpdateChunkSize(ChunkModel chunk) {
        chunk.Width = ViewportPixelPerChunk;
    }

    private void UpdateChunkPosition(ChunkModel chunk, PointF centerPoint) {
        PointF chunkPosOffset = ViewportChunkPosOffset;
        // scale from world origin is offset amount required to align the
        // coordinate to zoomed coordinate measured from world origin.
        // Here we are scaling the cartesian coordinate value by zoom amount
        // (which is pixel-per-chunk)
        Point scaleFromWorldOrigin = new(
            ViewportPixelPerChunk * chunk.CanvasPos.X,
            ViewportPixelPerChunk * chunk.CanvasPos.Y
        );

        // Push toward center is offset amount required to align the coordinate
        // relative to the chunk canvas center,
        // so it creates "zoom toward center" effect
        PointF pushTowardCenter = centerPoint;

        // Origin offset is offset amount requred to align the coordinate
        // to keep it stays aligned with moved world origin
        // when view is dragged around.
        // The offset itself is from camera position.
        // It is inverted because obviously, if camera is 1 meter to the right
        // of origin, then everything else the camera sees must be 1 meter
        // shifted to the left of the camera
        PointF originOffset = new(
            -chunkPosOffset.X,
            -chunkPosOffset.Y
        );

        PointF finalPos = scaleFromWorldOrigin + originOffset + pushTowardCenter;
        Canvas.SetLeft(chunk, finalPos.X);
        Canvas.SetTop(chunk, finalPos.Y);
    }

    private void OnMouseWheel(object sender, MouseWheelEventArgs e) {
        if (e.Delta > 0)
            ViewportZoomLevel++;
        else
            ViewportZoomLevel--;
        ViewportZoomLevel = Math.Clamp(ViewportZoomLevel, 1, s_zoomBlockPixelCount.Length);
        UpdateChunkTransformation(updateChunkSize: true);
        UpdateDebugPanel();
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs e) {
        // we don't update chunk size because we are not zooming,
        // we are just handling what happen when window is resized
        UpdateChunkTransformation(updateChunkSize: false);
    }

    private void OnMouseMove(object sender, MouseEventArgs e) {
        PointF oldMousePos = MousePos;
        PointF newMousePos = new(e.GetPosition(this).X,
                                 e.GetPosition(this).Y);

        // Set delta to 0 if this call is initial click and dragging.
        // This is to avoid jumps when clicking menu bar
        // then clicking and dragging on the viewer again.
        PointF newMousePosDelta = PointF.Origin;
        newMousePosDelta.X = MouseInitClickDrag && MouseIsClickHolding ? 0 : newMousePos.X - oldMousePos.X;
        newMousePosDelta.Y = MouseInitClickDrag && MouseIsClickHolding ? 0 : newMousePos.Y - oldMousePos.Y;
        MousePosDelta = newMousePosDelta; 
        MousePos = newMousePos;
        
        if (MouseIsClickHolding) {
            ViewportCameraPos -= MousePosDelta / ViewportPixelPerBlock;

            MouseInitClickDrag = false;
            UpdateChunkTransformation(updateChunkSize: false);
        }
        UpdateDebugPanel();
    }

    private void OnMouseUp(object sender, MouseButtonEventArgs e) {
        if (e.LeftButton == MouseButtonState.Released) {
            MouseIsClickHolding = false;
            MouseInitClickDrag = true;
        }
        UpdateDebugPanel();
    }

    private void OnMouseDown(object sender, MouseButtonEventArgs e) {
        if (e.LeftButton == MouseButtonState.Pressed) {
            MouseIsClickHolding = true;
        }
        UpdateDebugPanel();
    }

    private void OnMouseLeave(object sender, MouseEventArgs e) {
        MouseIsOutside = true;
        MouseIsClickHolding = false;
        UpdateDebugPanel();
    }

    private void OnMouseEnter(object sender, MouseEventArgs e) {
        MouseIsOutside = false;
        UpdateDebugPanel();
    }

    private void UpdateDebugPanel() {
        if (ViewportDebugControlViewModel.Context is ViewportDebugControlViewModel vm) {
            vm.CameraPos = $"Camera Pos: ({floor(ViewportCameraPos.X)}, {floor(ViewportCameraPos.Y)})";
            vm.ChunkPosOffset = $"Chunk Pos Offset: ({floor(ViewportChunkPosOffset.X)}, {floor(ViewportChunkPosOffset.Y)})";
            vm.ZoomLevel = $"Zoom Level: {ViewportZoomLevel}";
            vm.PixelPerBlock = $"Pixel-Per-Chunk: {ViewportPixelPerBlock}";
            vm.PixelPerChunk = $"Pixel-Per-Chunk: {ViewportPixelPerChunk}";

            vm.MousePos = $"Mouse Pos: ({floor(MousePos.X)}, {floor(MousePos.Y)})";
            vm.MousePosDelta = $"Mouse Pos Delta: ({floor(MousePosDelta.X)}, {floor(MousePosDelta.Y)})";
            vm.MouseIsClickHolding = $"Is Click Holding: {MouseIsClickHolding}";
            vm.MouseIsOutside = $"Is Outside: {MouseIsOutside}";
        }
        static double floor(double f) => Math.Floor(f);
    }
}
