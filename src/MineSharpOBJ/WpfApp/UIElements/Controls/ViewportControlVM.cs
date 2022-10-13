using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using binstarjs03.MineSharpOBJ.Core.CoordinateSystem;
using binstarjs03.MineSharpOBJ.Core.RegionMc;
using binstarjs03.MineSharpOBJ.WpfApp.Services;

namespace binstarjs03.MineSharpOBJ.WpfApp.UIElements.Controls;

public class ViewportControlVM : ViewModelBase<ViewportControlVM, ViewportControl>
{
    public ViewportControlVM(ViewportControl control) : base(control)
    {
        ChunkService = new ViewportChunkService(this);

        // listen to shared property change
        SharedProperty.PropertyChanged += OnSharedPropertyChanged;
        
        // TODO delete below sample code when chunk loading is stable
        for (int x = -8; x < 16; x++)
        {
            for (int y = -8; y < 16; y++)
            {
                PointInt2 chunkCanvasPos = new(x, y);
                ChunkImageControl chunk = new(chunkCanvasPos);
                chunk.SetRandomImage((x + y) % 2 == 0);
                Control.ChunkCanvas.Children.Add(chunk);
            }
        }
        ReinitializeStates();
    }

    // Accessors --------------------------------------------------------------
    
    private ViewportChunkService ChunkService { get; }

    public Size ViewportChunkCanvasSize => new(Control.ChunkCanvas.Width,
                                               Control.ChunkCanvas.Height);
    public PointF2 ViewportChunkCanvasCenter
        => new(Control.ChunkCanvas.ActualWidth / 2,
               Control.ChunkCanvas.ActualHeight / 2);

    // we can make this property as static, but XAML databinding
    // intellisense won't detect this property anymore
    public bool HasSession => SharedProperty.HasSession;

    private static readonly int[] s_zoomBlockPixelCount = new int[] {
        1, 2, 3, 5, 8, 13, 21, 34
    };

    // Data Binders -----------------------------------------------------------

    // Viewport Group

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

    private PointF2 _viewportCameraPos = PointF2.Zero;
    public PointF2 ViewportCameraPos
    {
        get => _viewportCameraPos;
        set => SetAndNotifyPropertyChanged(value, ref _viewportCameraPos);
    }

    public PointF2 ViewportChunkPosOffset
        => ViewportCameraPos * ViewportPixelPerBlock;

    private int _viewportZoomLevel = 3;
    public int ViewportZoomLevel
    {
        get => _viewportZoomLevel;
        set => SetAndNotifyPropertyChanged(value, ref _viewportZoomLevel);
    }

    public int ViewportPixelPerBlock => s_zoomBlockPixelCount[ViewportZoomLevel];

    public int ViewportPixelPerChunk => ViewportPixelPerBlock * Section.BlockCount;

    
    
    // Mouse Group

    private PointF2 _mousePos = PointF2.Zero;
    public PointF2 MousePos
    {
        get => _mousePos;
        set => SetAndNotifyPropertyChanged(value, ref _mousePos);
    }

    private PointF2 _mousePosDelta = PointF2.Zero;
    public PointF2 MousePosDelta
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
        ViewportCameraPos = PointF2.Zero;
        ViewportZoomLevel = 3;
        MousePos = PointF2.Zero;
        MousePosDelta = PointF2.Zero;
        MouseIsClickHolding = false;
        MouseInitClickDrag = true;
        MouseIsOutside = true;
        UpdateChunk(updateChunkSize: true);
    }

    public void SetCameraPosition(PointF2 cameraPos)
    {
        ViewportCameraPos = cameraPos;
        UpdateChunk(updateChunkSize: false);
        UpdateDebugPanel();
    }

    public void UpdateChunk(bool updateChunkSize)
    {
        ChunkService.UpdateVisibleChunkRange();
        PointF2 chunkCanvasCenter = new(
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

    private void UpdateChunkPosition(ChunkImageControl chunk, PointF2 centerPoint)
    {
        // we floor all the floating-point number here
        // so it snaps perfectly to the pixel and it removes "Jaggy-Moving"
        // illusion of the chunkimagecontrol.
        // Try to not floor it and see yourself the illusion

        PointF2 chunkPosOffset = ViewportChunkPosOffset.Floor;
        // scaled unit is offset amount required to align the
        // coordinate to zoomed coordinate measured from world origin.
        // Here we are scaling the cartesian coordinate unit by zoom amount
        // (which is pixel-per-chunk)
        PointInt2 scaledUnit = chunk.CanvasPos * ViewportPixelPerChunk;

        // Push toward center is offset amount required to align the coordinate
        // relative to the chunk canvas center,
        // so it creates "zoom toward center" effect
        PointF2 pushTowardCenter = centerPoint.Floor;
        //PointF pushTowardCenter = PointF.Origin;

        // Origin offset is offset amount requred to align the coordinate
        // to keep it stays aligned with moved world origin
        // when view is dragged around.
        // The offset itself is from camera position.
        // It is inverted because obviously, if camera is 1 meter to the right
        // of origin, then everything else the camera sees must be 1 meter
        // shifted to the left of the camera
        PointF2 originOffset = -chunkPosOffset;

        PointF2 finalPos 
            = (originOffset + scaledUnit + pushTowardCenter).Floor;
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
        UpdateChunk(updateChunkSize: false);
        UpdateDebugPanel();
    }

    public void OnMouseWheel(object sender, MouseWheelEventArgs e)
    {
        if (e.Delta > 0)
            ViewportZoomLevel++;
        else
            ViewportZoomLevel--;
        ViewportZoomLevel = Math.Clamp(ViewportZoomLevel, 0, s_zoomBlockPixelCount.Length - 1);
        UpdateChunk(updateChunkSize: true);
        UpdateDebugPanel();
    }

    public void OnMouseMove(object sender, MouseEventArgs e)
    {
        PointF2 oldMousePos = MousePos;
        PointF2 newMousePos = new(e.GetPosition(Control).X,
                                 e.GetPosition(Control).Y);

        // Set delta to 0 if this call is initial click and dragging.
        // This is to avoid jumps when clicking menu bar
        // then clicking and dragging on the viewer again.
        PointF2 newMousePosDelta = PointF2.Zero;
        newMousePosDelta.X = MouseInitClickDrag && MouseIsClickHolding ? 0 : newMousePos.X - oldMousePos.X;
        newMousePosDelta.Y = MouseInitClickDrag && MouseIsClickHolding ? 0 : newMousePos.Y - oldMousePos.Y;
        MousePosDelta = newMousePosDelta;
        MousePos = newMousePos;

        if (MouseIsClickHolding)
        {
            ViewportCameraPos -= MousePosDelta / ViewportPixelPerBlock;

            MouseInitClickDrag = false;
            UpdateChunk(updateChunkSize: false);
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

            vm.ChunkServiceVisibleChunkXRange = $"- X Range: {ChunkService.VisibleChunkRange.XRange}";
            vm.ChunkServiceVisibleChunkZRange = $"- Z Range: {ChunkService.VisibleChunkRange.ZRange}";
        }
        else
            throw new NullReferenceException($"{nameof(ViewportDebugInfoControlVM)} context is null");
        static double floor(double f) => Math.Floor(f);
    }
}
