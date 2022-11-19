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
public partial class ViewportControlVM : BaseViewModel, IViewport
{
    private const double s_zoomRatio = 1.5;

    [ObservableProperty] private Vector _cameraPos = new(0, 0);
    [ObservableProperty] private double _zoomLevel = 1;
    [ObservableProperty] private int _heightLimit = 319;
    [ObservableProperty] private Size _screenSize = new(0, 0);

    [ObservableProperty] private Point _mousePos = new(0, 0);
    [ObservableProperty] private Point _mousePosDelta = new(0, 0);
    [ObservableProperty] private bool _mouseClickHolding = false;
    [ObservableProperty] private bool _mouseInitClickDrag = true;
    [ObservableProperty] private bool _mouseIsOutside = true;

    public event Action? RequestFocusToViewport;

    public double UnitScale => _zoomLevel;
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


    public ViewportControlVM()
    {
        SharedStateService.ViewportSidePanelVisibilityChanged += OnSidePanelVisibilityChanged;
        SharedStateService.ViewportSidePanelDebugInfoVisibilityChanged += OnSidePanelDebugInfoVisibilityChanged;
    }

    private void OnSidePanelVisibilityChanged(bool obj) => OnPropertyChanged(nameof(IsSidePanelVisible));
    private void OnSidePanelDebugInfoVisibilityChanged(bool obj) => OnPropertyChanged(nameof(IsSidePanelDebugInfoVisible));

    [RelayCommand]
    private void OnScreenSizeChanged(SizeChangedEventArgs e)
    {
        Size newSize = e.NewSize.GetFloor();
        ScreenSize = newSize;
    }

    [RelayCommand]
    private void OnMouseMove(MouseEventArgs e)
    {
        Point oldMousePos = MousePos;
        Point newMousePos = e.GetPosition(e.Source as IInputElement).GetFloor();
        Point newMousePosDelta = ((Point)(newMousePos - oldMousePos)).GetFloor();
        MousePos = newMousePos;
        MousePosDelta = MouseInitClickDrag && MouseClickHolding ? new Point(0, 0) : newMousePosDelta;
        if (MouseClickHolding)
        {
            CameraPos -= ((Vector)MousePosDelta) / UnitScale;
            MouseInitClickDrag = false;
        }
    }

    [RelayCommand]
    private void OnMouseWheel(MouseWheelEventArgs e)
    {
        double newZoomLevel;
        if (e.Delta > 0)
            newZoomLevel = ZoomLevel * s_zoomRatio;
        else
            newZoomLevel = ZoomLevel / s_zoomRatio;
        // limit zoom scrollability by 8 for zoom in, 2 for zoom out
        ZoomLevel = Math.Clamp(newZoomLevel,
                               1 / Math.Pow(s_zoomRatio, 2),
                               1 * Math.Pow(s_zoomRatio, 8));
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
        RequestFocusToViewport?.Invoke();
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

    partial void OnZoomLevelChanged(double value)
    {
        OnPropertyChanged(nameof(PixelPerBlock));
        OnPropertyChanged(nameof(PixelPerChunk));
        OnPropertyChanged(nameof(PixelPerRegion));
    }
}
