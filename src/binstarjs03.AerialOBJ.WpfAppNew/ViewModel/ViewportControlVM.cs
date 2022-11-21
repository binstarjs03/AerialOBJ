using System;
using System.Windows;
using System.Windows.Input;

using binstarjs03.AerialOBJ.WpfAppNew.Components;
using binstarjs03.AerialOBJ.WpfAppNew.Components.Interfaces;
using binstarjs03.AerialOBJ.WpfAppNew.Services;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Range = binstarjs03.AerialOBJ.Core.Range;

namespace binstarjs03.AerialOBJ.WpfAppNew.ViewModel;

#pragma warning disable CA1822 // Mark members as static
public partial class ViewportControlVM : BaseViewModel
{
    private const double s_zoomRatio = 1.5;
    private readonly IViewportView _viewportView;
    private readonly ChunkRegionViewport _viewport = new();

    [ObservableProperty] private Point _mousePos = new(0, 0);
    [ObservableProperty] private Vector _mousePosDelta = new(0, 0);
    [ObservableProperty] private bool _mouseClickHolding = false;
    [ObservableProperty] private bool _mouseInitClickDrag = true;
    [ObservableProperty] private bool _mouseIsOutside = true;

    #region Properties
    public Point CameraPos => _viewport.CameraPos;
    public double ZoomLevel => _viewport.ZoomLevel;
    public Size ScreenSize => _viewport.ScreenSize;
    public Point ScreenCenter => _viewport.ScreenCenter;
    public double PixelPerBlock => _viewport.PixelPerBlock;
    public double PixelPerChunk => _viewport.PixelPerChunk;
    public double PixelPerRegion => _viewport.PixelPerRegion;
    public int HeightLimit => _viewport.HeightLimit;
    public Range VisibleRegionXRange => _viewport.VisibleRegionRange.XRange;
    public Range VisibleRegionZRange => _viewport.VisibleRegionRange.ZRange;
    public Range VisibleChunkXRange => _viewport.VisibleChunkRange.XRange;
    public Range VisibleChunkZRange => _viewport.VisibleChunkRange.ZRange;
    public bool IsSidePanelVisible
    {
        get => SharedStateService.IsViewportSidePanelVisible;
        set => SharedStateService.IsViewportSidePanelVisible = value;
    }
    public bool IsSidePanelDebugInfoVisible
    {
        get => SharedStateService.IsViewportSidePanelDebugInfoVisible;
        set => SharedStateService.IsViewportSidePanelDebugInfoVisible = value;
    }
    #endregion Properties

    public ViewportControlVM(IViewportView viewportView)
    {
        SharedStateService.ViewportSidePanelVisibilityChanged += OnSidePanelVisibilityChanged;
        SharedStateService.ViewportSidePanelDebugInfoVisibilityChanged += OnSidePanelDebugInfoVisibilityChanged;
        _viewport.CameraPosChanged += OnViewportCameraPosChanged;
        _viewport.ZoomLevelChanged += OnViewportZoomLevelChanged;
        _viewport.ScreenSizeChanged += OnViewportScreenSizeChanged;
        _viewport.ScreenCenterChanged += OnViewportScreenCenterChanged;
        _viewport.PixelPerBlockChanged += OnViewportPixelPerBlockChanged;
        _viewport.PixelPerChunkChanged += OnViewportPixelPerChunkChanged;
        _viewport.PixelPerRegionChanged += OnViewportPixelPerRegionChanged;
        _viewport.HeightLimitChanged += OnViewportHeightLimitChanged;
        _viewport.VisibleRegionRangeChanged += OnViewportVisibleRegionRangeChanged;
        _viewport.VisibleChunkRangeChanged += OnViewportVisibleChunkRangeChanged;
        _viewportView = viewportView;
    }

    #region Event Handlers
    private void OnSidePanelVisibilityChanged(bool obj) => OnPropertyChanged(nameof(IsSidePanelVisible));
    private void OnSidePanelDebugInfoVisibilityChanged(bool obj) => OnPropertyChanged(nameof(IsSidePanelDebugInfoVisible));
    private void OnViewportCameraPosChanged() => OnPropertyChanged(nameof(CameraPos));
    private void OnViewportZoomLevelChanged() => OnPropertyChanged(nameof(ZoomLevel));
    private void OnViewportScreenSizeChanged() => OnPropertyChanged(nameof(ScreenSize));
    private void OnViewportScreenCenterChanged() => OnPropertyChanged(nameof(ScreenCenter));
    private void OnViewportHeightLimitChanged() => OnPropertyChanged(nameof(HeightLimit));
    private void OnViewportPixelPerBlockChanged() => OnPropertyChanged(nameof(PixelPerBlock));
    private void OnViewportPixelPerChunkChanged() => OnPropertyChanged(nameof(PixelPerChunk));
    private void OnViewportPixelPerRegionChanged() => OnPropertyChanged(nameof(PixelPerRegion));
    private void OnViewportVisibleRegionRangeChanged()
    {
        OnPropertyChanged(nameof(VisibleRegionXRange));
        OnPropertyChanged(nameof(VisibleRegionZRange));
    }
    private void OnViewportVisibleChunkRangeChanged()
    {
        OnPropertyChanged(nameof(VisibleChunkXRange));
        OnPropertyChanged(nameof(VisibleChunkZRange));
    }
    #endregion Event Handlers

    #region Relay Commands
    [RelayCommand]
    private void OnScreenSizeChanged(SizeChangedEventArgs e)
    {
        Size newSize = e.NewSize.GetFloor();
        _viewport.ScreenSize = newSize;
    }

    [RelayCommand]
    private void OnMouseMove(MouseEventArgs e)
    {
        Point oldMousePos = MousePos;
        Point newMousePos = e.GetPosition(e.Source as IInputElement).GetFloor();
        Vector newMousePosDelta = (newMousePos - oldMousePos).GetFloor();
        MousePos = newMousePos;
        MousePosDelta = MouseInitClickDrag && MouseClickHolding ? new Vector(0, 0) : newMousePosDelta;
        if (MouseClickHolding)
        {
            Vector cameraPosDelta = MousePosDelta / _viewport.ZoomLevel;
            _viewport.CameraPos += cameraPosDelta;
            MouseInitClickDrag = false;
        }
    }

    [RelayCommand]
    private void OnMouseWheel(MouseWheelEventArgs e)
    {
        double newZoomLevel;
        if (e.Delta > 0)
            newZoomLevel = _viewport.ZoomLevel * s_zoomRatio;
        else
            newZoomLevel = _viewport.ZoomLevel / s_zoomRatio;
        // limit zoom scrollability by 8 for zoom in, 2 for zoom out
        newZoomLevel = Math.Clamp(newZoomLevel,
                               1 / Math.Pow(s_zoomRatio, 2),
                               1 * Math.Pow(s_zoomRatio, 8));
        _viewport.ZoomLevel = newZoomLevel;
    }

    [RelayCommand]
    private void OnMouseUp(MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Released)
        {
            MouseClickHolding = false;
            MouseInitClickDrag = true;
        }
    }

    [RelayCommand]
    private void OnMouseDown(MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
            MouseClickHolding = true;
    }

    [RelayCommand]
    private void OnMouseEnter()
    {
        MouseIsOutside = false;
        _viewportView.Focus();
    }

    [RelayCommand]
    private void OnMouseLeave()
    {
        MouseIsOutside = true;
        MouseClickHolding = false;
    }

    [RelayCommand]
    private void OnKey(KeyEventArgs e)
    {
        MessageBox.Show("Key Up!");
    }
    #endregion Relay Commands
}
