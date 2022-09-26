using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using binstarjs03.MineSharpOBJ.Core.RegionMc;
using binstarjs03.MineSharpOBJ.Core.Utils;

using Point = binstarjs03.MineSharpOBJ.Core.Utils.Point;

namespace binstarjs03.MineSharpOBJ.WpfApp.UIElements.Controls;

public class ViewportControlVM : ViewModelBase<ViewportControlVM, ViewportControl>
{
    public ViewportControlVM(ViewportControl control) : base(control)
    {
        // listen to shared property change
        SharedProperty.PropertyChanged += OnSharedPropertyChanged;
        
        // TODO delete below sample code when chunk loading is stable
        for (int x = -8; x < 16; x++)
        {
            for (int y = -8; y < 16; y++)
            {
                ChunkImageControl chunk = new(new(x, y));
                chunk.SetRandomImage((x + y) % 2 == 0);
                Control.ChunkCanvas.Children.Add(chunk);
            }
        }
        ReinitializeStates();
    }

    // States -----------------------------------------------------------------

    private static readonly int[] s_zoomBlockPixelCount = new int[] {
        1, 2, 3, 5, 8, 13, 21, 34
    };

    public bool IsViewportCameraPositionGuideVisible
    {
        get => SharedProperty.IsViewportCameraPositionGuideVisible;
        set => SetSharedPropertyChanged
        (
            value,
            SharedProperty.IsViewportCameraPositionGuideVisibleUpdater
        );
    }

    public bool IsViewportDebugInfoVisible
    {
        get => SharedProperty.IsViewportDebugInfoVisible;
        set => SetSharedPropertyChanged
        (
            value,
            SharedProperty.IsViewportDebugInfoVisibleUpdater
        );
    }

    private PointF _viewportCameraPos = PointF.Origin;
    public PointF ViewportCameraPos
    {
        get => _viewportCameraPos;
        set => SetAndNotifyPropertyChanged(value, ref _viewportCameraPos);
    }

    public PointF ViewportChunkPosOffset
        => ViewportCameraPos * ViewportPixelPerBlock;

    private int _viewportZoomLevel = 2;
    public int ViewportZoomLevel
    {
        get => _viewportZoomLevel;
        set => SetAndNotifyPropertyChanged(value, ref _viewportZoomLevel);
    }

    public int ViewportPixelPerBlock => s_zoomBlockPixelCount[ViewportZoomLevel];

    public int ViewportPixelPerChunk => ViewportPixelPerBlock * Section.BlockCount;



    private PointF _mousePos = PointF.Origin;
    public PointF MousePos
    {
        get => _mousePos;
        set => SetAndNotifyPropertyChanged(value, ref _mousePos);
    }

    private PointF _mousePosDelta = PointF.Origin;
    public PointF MousePosDelta
    {
        get => _mousePosDelta;
        set => SetAndNotifyPropertyChanged(value, ref _mousePosDelta);
    }

    private bool _mouseIsClickHolding = false;
    public bool MouseIsClickHolding
    {
        get => _mouseIsClickHolding;
        set => SetAndNotifyPropertyChanged(value, ref _mouseIsClickHolding);
    }

    private bool _mouseInitClickDrag = true;
    public bool MouseInitClickDrag
    {
        get => _mouseInitClickDrag;
        set => SetAndNotifyPropertyChanged(value, ref _mouseInitClickDrag);
    }

    private bool _mouseIsOutside = true;
    public bool MouseIsOutside
    {
        get => _mouseIsOutside;
        set => SetAndNotifyPropertyChanged(value, ref _mouseIsOutside);
    }

    // Methods ----------------------------------------------------------------

    private void ReinitializeStates()
    {
        ViewportCameraPos = PointF.Origin;
        ViewportZoomLevel = 2;
        MousePos = PointF.Origin;
        MousePosDelta = PointF.Origin;
        MouseIsClickHolding = false;
        MouseInitClickDrag = true;
        MouseIsOutside = true;
        UpdateChunkTransformation(updateChunkSize: true);
    }

    public void SetCameraPosition(PointF cameraPos)
    {
        ViewportCameraPos = cameraPos;
        UpdateChunkTransformation(updateChunkSize: false);
        UpdateDebugPanel();
    }

    public void UpdateChunkTransformation(bool updateChunkSize)
    {
        PointF chunkCanvasCenter = new(
            Control.ChunkCanvas.ActualWidth / 2,
            Control.ChunkCanvas.ActualHeight / 2
        );
        foreach (ChunkImageControl chunk in Control.ChunkCanvas.Children)
        {
            if (updateChunkSize)
                UpdateChunkSize(chunk);
            UpdateChunkPosition(chunk, chunkCanvasCenter);
        }
    }

    private void UpdateChunkSize(ChunkImageControl chunk)
    {
        chunk.Width = ViewportPixelPerChunk;
    }

    private void UpdateChunkPosition(ChunkImageControl chunk, PointF centerPoint)
    {
        // we floor all the floating-point number here
        // so it snaps perfectly to the pixel and it removes "Jaggy-Moving"
        // illusion of the chunkimagecontrol.
        // Try to not floor it and see yourself the illusion

        PointF chunkPosOffset = ViewportChunkPosOffset.Floor;
        // scale from world origin is offset amount required to align the
        // coordinate to zoomed coordinate measured from world origin.
        // Here we are scaling the cartesian coordinate value by zoom amount
        // (which is pixel-per-chunk)
        Point scaleFromWorldOrigin = chunk.CanvasPos * ViewportPixelPerChunk;

        // Push toward center is offset amount required to align the coordinate
        // relative to the chunk canvas center,
        // so it creates "zoom toward center" effect
        PointF pushTowardCenter = centerPoint.Floor;

        // Origin offset is offset amount requred to align the coordinate
        // to keep it stays aligned with moved world origin
        // when view is dragged around.
        // The offset itself is from camera position.
        // It is inverted because obviously, if camera is 1 meter to the right
        // of origin, then everything else the camera sees must be 1 meter
        // shifted to the left of the camera
        PointF originOffset = chunkPosOffset.Negative;

        PointF finalPos 
            = (scaleFromWorldOrigin + originOffset + pushTowardCenter).Floor;
        Canvas.SetLeft(chunk, finalPos.X);
        Canvas.SetTop(chunk, finalPos.Y);
    }

    // Event Handlers ---------------------------------------------------------

    protected override void OnSharedPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        base.OnSharedPropertyChanged(sender, e);
        if (e.PropertyName! == nameof(SharedProperty.SessionInfo))
        {
            ReinitializeStates();
        }
    }

    public void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        // we don't update chunk size because we are not zooming,
        // we are just handling what happen when window is resized
        UpdateChunkTransformation(updateChunkSize: false);
    }

    public void OnMouseWheel(object sender, MouseWheelEventArgs e)
    {
        if (e.Delta > 0)
            ViewportZoomLevel++;
        else
            ViewportZoomLevel--;
        ViewportZoomLevel = Math.Clamp(ViewportZoomLevel, 0, s_zoomBlockPixelCount.Length - 1);
        UpdateChunkTransformation(updateChunkSize: true);
        UpdateDebugPanel();
    }

    public void OnMouseMove(object sender, MouseEventArgs e)
    {
        PointF oldMousePos = MousePos;
        PointF newMousePos = new(e.GetPosition(Control).X,
                                 e.GetPosition(Control).Y);

        // Set delta to 0 if this call is initial click and dragging.
        // This is to avoid jumps when clicking menu bar
        // then clicking and dragging on the viewer again.
        PointF newMousePosDelta = PointF.Origin;
        newMousePosDelta.X = MouseInitClickDrag && MouseIsClickHolding ? 0 : newMousePos.X - oldMousePos.X;
        newMousePosDelta.Y = MouseInitClickDrag && MouseIsClickHolding ? 0 : newMousePos.Y - oldMousePos.Y;
        MousePosDelta = newMousePosDelta;
        MousePos = newMousePos;

        if (MouseIsClickHolding)
        {
            ViewportCameraPos -= MousePosDelta / ViewportPixelPerBlock;

            MouseInitClickDrag = false;
            UpdateChunkTransformation(updateChunkSize: false);
        }
        UpdateDebugPanel();
    }

    public void OnMouseUp(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Released)
        {
            MouseIsClickHolding = false;
            MouseInitClickDrag = true;
        }
        UpdateDebugPanel();
    }

    public void OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            MouseIsClickHolding = true;
        }
        UpdateDebugPanel();
    }

    public void OnMouseLeave(object sender, MouseEventArgs e)
    {
        MouseIsOutside = true;
        MouseIsClickHolding = false;
        UpdateDebugPanel();
    }

    public void OnMouseEnter(object sender, MouseEventArgs e)
    {
        MouseIsOutside = false;
        UpdateDebugPanel();
    }

    private void UpdateDebugPanel()
    {
        if (ViewportDebugInfoControlVM.Context is ViewportDebugInfoControlVM vm)
        {
            vm.ViewportCameraPos = $"Camera Pos: ({floor(ViewportCameraPos.X)}, {floor(ViewportCameraPos.Y)})";
            vm.ViewportChunkPosOffset = $"Chunk Pos Offset: ({floor(ViewportChunkPosOffset.X)}, {floor(ViewportChunkPosOffset.Y)})";
            vm.ViewportZoomLevel = $"Zoom Level: {ViewportZoomLevel}";
            vm.ViewportPixelPerBlock = $"Pixel-Per-Chunk: {ViewportPixelPerBlock}";
            vm.ViewportPixelPerChunk = $"Pixel-Per-Chunk: {ViewportPixelPerChunk}";

            vm.MousePos = $"Mouse Pos: ({floor(MousePos.X)}, {floor(MousePos.Y)})";
            vm.MousePosDelta = $"Mouse Pos Delta: ({floor(MousePosDelta.X)}, {floor(MousePosDelta.Y)})";
            vm.MouseIsClickHolding = $"Is Click Holding: {MouseIsClickHolding}";
            vm.MouseIsOutside = $"Is Outside: {MouseIsOutside}";
        }
        else
            throw new NullReferenceException($"{nameof(ViewportDebugInfoControlVM)} context is null");
        static double floor(double f) => Math.Floor(f);
    }
}
