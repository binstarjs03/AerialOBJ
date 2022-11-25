using System;
using System.Windows;
using System.Windows.Controls;
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
    [ObservableProperty] private Point2<int> _mousePos = Point2<int>.Zero;
    [ObservableProperty] private Vector2<int> _mousePosDelta = Vector2<int>.Zero;
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
    public int LoadedRegionCount => _viewport.LoadedRegionCount;
    public int PendingRegionCount => _viewport.PendingRegionCount;
    public string WorkedRegion => _viewport.WorkedRegion is null ? "None" : _viewport.WorkedRegion.Value.ToString();

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
        App.Current.Initializing += OnAppInitializing;
        SharedStateService.SavegameLoadChanged += OnSavegameLoadChanged;
        SharedStateService.ViewportSidePanelVisibilityChanged += OnSidePanelVisibilityChanged;
        SharedStateService.ViewportSidePanelDebugInfoVisibilityChanged += OnSidePanelDebugInfoVisibilityChanged;
        _viewportView = viewportView;
        _viewport = new ChunkRegionViewport();
        _viewport.RegionImageLoaded += OnViewportRegionImageLoaded;
        _viewport.RegionImageUnloaded += OnViewportRegionImageUnloaded;
        _viewport.PropertyChanged += OnViewportPropertyChanged;
    }

    private void OnSavegameLoadChanged(SavegameLoadState state)
    {
        _viewport.PostMessage(_viewport.Reinitialize, MessageOption.NoDuplicate);
        _viewport.PostMessage(()=>_viewport.Update(true), MessageOption.NoDuplicate);
    }

    private void OnAppInitializing(object sender, StartupEventArgs e)
    {
        _viewport.PostMessage(() => _viewport.ScreenSize = _viewportView.GetScreenSize().ToCoreSize(),
                              MessageOption.NoDuplicate);
    }


    #region Event Handlers
    private void OnSidePanelVisibilityChanged(bool obj)
    {
        OnPropertyChanged(nameof(IsSidePanelVisible));
    }

    private void OnSidePanelDebugInfoVisibilityChanged(bool obj)
    {
        OnPropertyChanged(nameof(IsSidePanelDebugInfoVisible));
    }

    private void OnViewportRegionImageUnloaded(Image regionImage)
    {
        _viewportView.RemoveFromCanvas(regionImage);
    }

    private void OnViewportRegionImageLoaded(Image regionImage)
    {
        _viewportView.AddToCanvas(regionImage);
    }

    private void OnViewportPropertyChanged(string obj)
    {
        OnPropertyChanged(obj);
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
    }
    #endregion Event Handlers

    #region Relay Commands
    [RelayCommand]
    private void OnScreenSizeChanged(SizeChangedEventArgs e)
    {
        Size<int> newSize = e.NewSize.GetFloor().ToCoreSize();
        _viewport.PostMessage(() => _viewport.ScreenSize = newSize, MessageOption.NoDuplicate);
    }

    [RelayCommand]
    private void OnMouseMove(MouseEventArgs e)
    {
        Point2<int> oldMousePos = MousePos;
        Point2<int> newMousePos = e.GetPosition(e.Source as IInputElement).GetFloor().ToCorePoint2();
        Vector2<int> newMousePosDelta = newMousePos - oldMousePos;
        MousePos = newMousePos;
        MousePosDelta = MouseInitClickDrag && MouseClickHolding ? Vector2<int>.Zero : newMousePosDelta;
        if (MouseClickHolding)
        {
            Vector2Z<float> cameraPosDelta = new(-MousePosDelta.X / _viewport.ZoomLevel, -MousePosDelta.Y / _viewport.ZoomLevel);
            _viewport.PostMessage(() => _viewport.CameraPos += cameraPosDelta, MessageOption.NoDuplicate);
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
        newZoomLevel = float.Clamp(newZoomLevel, 1, 1 * MathF.Pow(s_zoomRatio, 8));
        _viewport.PostMessage(() => _viewport.ZoomLevel = newZoomLevel, MessageOption.NoDuplicate);
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
