using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using binstarjs03.MineSharpOBJ.WpfApp.Models;
using binstarjs03.MineSharpOBJ.WpfApp.Services;
using binstarjs03.MineSharpOBJ.WpfApp.ViewModels.Controls;

using Point = binstarjs03.MineSharpOBJ.Core.Utils.Point;
using PointF = binstarjs03.MineSharpOBJ.Core.Utils.PointF;
using Section = binstarjs03.MineSharpOBJ.Core.RegionMc.Section;

namespace binstarjs03.MineSharpOBJ.WpfApp.Views.Controls;

// TODO draw in separate thread instead in UI thread (Main thread)

// TODO UX BUG inconvenience: when ClickDrag goes outside window, it loses track of where the mouse is,
// but when it goes inside back, it continue tracking the mouse, even when mouse click is release!!!

public partial class ViewportControl : UserControl {
    private static readonly int[] s_zoomBlockPixelCount = new int[] {
        1, 2, 3, 5, 8, 13, 21, 34
    };
    private PointF _viewportCameraPos = PointF.Origin;
    private int _viewportZoomLevel = 2;

    private PointF _mousePos = PointF.Origin;
    private PointF _mousePosDelta = PointF.Origin;
    private bool _mouseIsClickHolding = false;
    private bool _mouseInitClickDrag = true;

    private bool _isUpdatingDebug;

    public ViewportControl() {
        InitializeComponent();
        ViewportControlViewModel vm = new(this);
        ViewportControlViewModel.Context = vm;
        DataContext = vm;

        for (int x = -8; x < 16; x++) {
            for (int y = -8; y < 16; y++) {
                ChunkModel chunk = new(new(x, y));
                chunk.SetRandomImage((x + y) % 2 == 0);
                ChunkCanvas.Children.Add(chunk);
                chunk.Width = ViewportPixelPerChunk;
            }
        }
    }

    public PointF ViewportCameraPos => _viewportCameraPos;
    public PointF ViewportCameraPosZoomed => ViewportCameraPos * ViewportPixelPerBlock;
    public int ViewportZoomLevel => _viewportZoomLevel;
    public int ViewportPixelPerBlock => s_zoomBlockPixelCount[_viewportZoomLevel - 1];
    public int ViewportPixelPerChunk => ViewportPixelPerBlock * Section.BlockCount;

    public PointF MousePos => _mousePos;
    public PointF MousePosDelta => _mousePosDelta;

    public void SetCameraPosition(PointF cameraPos) {
        _viewportCameraPos = cameraPos;
        UpdateChunkTransformation(updateChunkSize: false);
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
        PointF cameraPosZoomed = ViewportCameraPosZoomed;
        // Push origin is offset amount required to align the coordinate
        // to zoomed coordinate measured from world origin
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
        PointF originOffset = cameraPosZoomed;

        PointF finalPos = scaleFromWorldOrigin + originOffset + pushTowardCenter;
        Canvas.SetLeft(chunk, finalPos.X);
        Canvas.SetTop(chunk, finalPos.Y);
    }

    private void OnMouseWheel(object sender, MouseWheelEventArgs e) {
        if (e.Delta > 0)
            _viewportZoomLevel++;
        else
            _viewportZoomLevel--;
        _viewportZoomLevel = Math.Clamp(_viewportZoomLevel, 1, s_zoomBlockPixelCount.Length);
        UpdateChunkTransformation(updateChunkSize: true);
        UpdateDebugPanel();
    }
    private void OnSizeChanged(object sender, SizeChangedEventArgs e) {
        // we don't update chunk size because we are not zooming,
        // we are just handling what happen when window is resized
        UpdateChunkTransformation(updateChunkSize: false);
    }

    private async void OnMouseMove(object sender, MouseEventArgs e) {
        // set delta to 0 if this call is initial click and dragging.
        // this is to avoid jumps when clicking menu bar
        // then clicking and dragging on the viewer again
        PointF mousePos = new(e.GetPosition(this).X,
                              e.GetPosition(this).Y);
        _mousePosDelta.X = _mouseInitClickDrag ? 0 : mousePos.X - _mousePos.X;
        _mousePosDelta.Y = _mouseInitClickDrag ? 0 : mousePos.Y - _mousePos.Y;
        _mousePos = mousePos;

        if (_mouseIsClickHolding) {
            _viewportCameraPos += (_mousePosDelta / ViewportPixelPerBlock);
            _mouseInitClickDrag = false;
            UpdateChunkTransformation(updateChunkSize: false);
        }
        if (!_isUpdatingDebug) {
            _isUpdatingDebug = true;
            await Task.Run(() => {
                Thread.Sleep(50);
                UpdateDebugPanel();
            });
            _isUpdatingDebug = false;
        }
    }

    private void OnMouseUp(object sender, MouseButtonEventArgs e) {
        if (e.LeftButton == MouseButtonState.Released) {
            _mouseIsClickHolding = false;
            _mouseInitClickDrag = true;
        }
        UpdateDebugPanel();
    }

    private void OnMouseDown(object sender, MouseButtonEventArgs e) {
        if (e.LeftButton == MouseButtonState.Pressed) {
            _mouseIsClickHolding = true;
        }
        UpdateDebugPanel();
    }

    private void UpdateDebugPanel() {
        if (ViewportDebugControlViewModel.Context is ViewportDebugControlViewModel vm) {
            vm.CameraPos = $"ViewPos: ({round(ViewportCameraPos.X)}, {round(ViewportCameraPos.Y)})";
            vm.CameraPosZoomed = $"ViewPos (zoomed): ({round(ViewportCameraPosZoomed.X)}, {round(ViewportCameraPosZoomed.Y)})";
            vm.ZoomLevel = $"Zoom Level: {ViewportZoomLevel}";
            vm.PixelPerBlock = $"Pixel-Per-Chunk: {ViewportPixelPerBlock}";
            vm.PixelPerChunk = $"Pixel-Per-Chunk: {ViewportPixelPerChunk}";

            vm.MousePos = $"MousePos: ({toint(MousePos.X)}, {toint(MousePos.Y)})";
            vm.MousePosDelta = $"MousePosDelta: ({toint(MousePosDelta.X)}, {toint(MousePosDelta.Y)})";
            vm.MouseIsClickHolding = $"MouseIsClickHolding: {_mouseIsClickHolding}";
        }
        static double round(double f) => Math.Round(f, 2);
        static int toint(double f) => (int)f;
    }
}
