using System;
using System.Windows;
using System.Windows.Input;

using binstarjs03.AerialOBJ.Core.Primitives;
using binstarjs03.AerialOBJ.WpfAppNew.Components;
using binstarjs03.AerialOBJ.WpfAppNew.Components.Interfaces;
using binstarjs03.AerialOBJ.WpfAppNew.Services;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace binstarjs03.AerialOBJ.WpfAppNew.ViewModel;

#pragma warning disable CA1822 // Mark members as static
public partial class ViewportControlVM : BaseViewModel
{
    private const float s_zoomRatio = 1.5f;
    private readonly IViewportView _viewportView;
    private readonly ChunkRegionViewport _viewport;

    // Here we are using native WPF primitives for better WPF integration
    // (less type casting around WPF types, cast only once when passing to the viewport)
    [ObservableProperty] private Point _mousePos = new(0, 0);
    [ObservableProperty] private Vector _mousePosDelta = new(0, 0);
    [ObservableProperty] private bool _mouseClickHolding = false;
    [ObservableProperty] private bool _mouseInitClickDrag = true;
    [ObservableProperty] private bool _mouseIsOutside = true;

    #region Properties
    public Point2Z<float> CameraPos => _viewport.CameraPos;
    public double ZoomLevel => _viewport.ZoomLevel;
    public Size<int> ScreenSize => _viewport.ScreenSize;
    public Point2<int> ScreenCenter => _viewport.ScreenCenter;

    public double PixelPerBlock => _viewport.PixelPerBlock;
    public double PixelPerChunk => _viewport.PixelPerChunk;
    public double PixelPerRegion => _viewport.PixelPerRegion;

    public Rangeof<int> VisibleRegionXRange => _viewport.VisibleRegionRange.XRange;
    public Rangeof<int> VisibleRegionZRange => _viewport.VisibleRegionRange.ZRange;
    public int PendingRegionCount => _viewport.PendingRegionCount;

    public Rangeof<int> VisibleChunkXRange => _viewport.VisibleChunkRange.XRange;
    public Rangeof<int> VisibleChunkZRange => _viewport.VisibleChunkRange.ZRange;
    public int PendingChunkCount => _viewport.PendingChunkCount;

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
        _viewportView = viewportView;
        _viewport = new ChunkRegionViewport(_viewportView.GetScreenSize().ToCoreSize());
        _viewport.PropertyChanged += OnViewportPropertyChanged;
    }

    private void OnViewportPropertyChanged(string obj)
    {
        if (obj == nameof(ChunkRegionViewport.VisibleRegionRange))
        {
            OnPropertyChanged(nameof(VisibleRegionXRange));
            OnPropertyChanged(nameof(VisibleRegionZRange));
        }
        else if (obj == nameof(ChunkRegionViewport.VisibleChunkRange))
        {
            OnPropertyChanged(nameof(VisibleChunkXRange));
            OnPropertyChanged(nameof(VisibleChunkZRange));
        }
        else
            OnPropertyChanged(obj);
    }

    #region Event Handlers
    private void OnSidePanelVisibilityChanged(bool obj) => OnPropertyChanged(nameof(IsSidePanelVisible));
    private void OnSidePanelDebugInfoVisibilityChanged(bool obj) => OnPropertyChanged(nameof(IsSidePanelDebugInfoVisible));
    #endregion Event Handlers

    #region Relay Commands
    [RelayCommand]
    private void OnScreenSizeChanged(SizeChangedEventArgs e)
    {
        Size newSize = e.NewSize.GetFloor();
        _viewport.ScreenSize = newSize.ToCoreSize();
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
            _viewport.CameraPos += cameraPosDelta.ToCoreVectorZ();
            MouseInitClickDrag = false;
        }
    }

    [RelayCommand]
    private void OnMouseWheel(MouseWheelEventArgs e)
    {
        float newZoomLevel;
        if (e.Delta > 0)
            newZoomLevel = _viewport.ZoomLevel * s_zoomRatio;
        else
            newZoomLevel = _viewport.ZoomLevel / s_zoomRatio;
        // limit zoom scrollability by 8 for zoom in, 2 for zoom out
        newZoomLevel = float.Clamp(newZoomLevel,
                                   1 / MathF.Pow(s_zoomRatio, 2),
                                   1 * MathF.Pow(s_zoomRatio, 8));
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
