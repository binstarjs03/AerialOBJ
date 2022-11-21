using System;
using System.Windows;
using System.Windows.Input;

using binstarjs03.AerialOBJ.Core.MinecraftWorld;
using binstarjs03.AerialOBJ.WpfAppNew.Components;
using binstarjs03.AerialOBJ.WpfAppNew.Components.Interfaces;
using binstarjs03.AerialOBJ.WpfAppNew.Services;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

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

    public Point CameraPos => _viewport.CameraPos;
    public double ZoomLevel => _viewport.ZoomLevel;
    public Size ScreenSize => _viewport.ScreenSize;
    public int HeightLimit => _viewport.HeightLimit;
    public double UnitScale => _viewport.ZoomLevel;
    public double PixelPerBlock => UnitScale;
    public double PixelPerChunk => PixelPerBlock * Section.BlockCount;
    public double PixelPerRegion => PixelPerChunk * Region.ChunkCount;

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


    public ViewportControlVM(IViewportView viewportView)
    {
        SharedStateService.ViewportSidePanelVisibilityChanged += OnSidePanelVisibilityChanged;
        SharedStateService.ViewportSidePanelDebugInfoVisibilityChanged += OnSidePanelDebugInfoVisibilityChanged;
        _viewport.CameraPosChanged += OnViewportCameraPosChanged;
        _viewport.ZoomLevelChanged += OnViewportZoomLevelChanged;
        _viewport.ScreenSizeChanged += OnViewportScreenSizeChanged;
        _viewport.HeightLimitChanged += OnViewportHeightLimitChanged;
        _viewportView = viewportView;
    }

    private void OnViewportCameraPosChanged() => OnPropertyChanged(nameof(CameraPos));
    private void OnViewportZoomLevelChanged()
    {
        OnPropertyChanged(nameof(ZoomLevel));
        OnPropertyChanged(nameof(UnitScale));
        OnPropertyChanged(nameof(PixelPerBlock));
        OnPropertyChanged(nameof(PixelPerChunk));
        OnPropertyChanged(nameof(PixelPerRegion));
    }
    private void OnViewportScreenSizeChanged() => OnPropertyChanged(nameof(ScreenSize));
    private void OnViewportHeightLimitChanged() => OnPropertyChanged(nameof(HeightLimit));

    private void OnSidePanelVisibilityChanged(bool obj) => OnPropertyChanged(nameof(IsSidePanelVisible));
    private void OnSidePanelDebugInfoVisibilityChanged(bool obj) => OnPropertyChanged(nameof(IsSidePanelDebugInfoVisible));

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
}
