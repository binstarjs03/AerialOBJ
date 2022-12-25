using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

using binstarjs03.AerialOBJ.Core;
using binstarjs03.AerialOBJ.Core.Primitives;
using binstarjs03.AerialOBJ.WpfAppNew2.Components;
using binstarjs03.AerialOBJ.WpfAppNew2.Factories;
using binstarjs03.AerialOBJ.WpfAppNew2.Models;
using binstarjs03.AerialOBJ.WpfAppNew2.Services;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace binstarjs03.AerialOBJ.WpfAppNew2.ViewModels;
[ObservableObject]
public partial class ViewportViewModel : IViewportViewModel
{
    private readonly float[] _zoomTable = new float[] { 1, 2, 3, 5, 8, 13, 21, 34 };
    private readonly RegionImageModelFactory _regionImageModelFactory;
    private readonly IChunkRegionManagerService _chunkRegionManagerService;

    [ObservableProperty] private Size<int> _screenSize = new(1, 1);
    [ObservableProperty] private Point2Z<float> _cameraPos = Point2Z<float>.Zero;
    [ObservableProperty] [NotifyPropertyChangedFor(nameof(UnitMultiplier))] private int _zoomLevel = 0;

    [ObservableProperty] private Point2<int> _mousePos = Point2<int>.Zero;
    [ObservableProperty] private Vector2<int> _mousePosDelta = Vector2<int>.Zero;
    [ObservableProperty] private bool _mouseClickHolding = false;
    [ObservableProperty] private bool _mouseInitClickDrag = true;
    [ObservableProperty] private bool _mouseIsInside = false;

    [ObservableProperty] private ObservableCollection<RegionImageModel> _regionImageModels = new();

    public ViewportViewModel(GlobalState globalState, RegionImageModelFactory regionImageModelFactory, IChunkRegionManagerService chunkRegionManagerService)
    {
        GlobalState = globalState;
        _regionImageModelFactory = regionImageModelFactory;
        _chunkRegionManagerService = chunkRegionManagerService;

        GlobalState.PropertyChanged += OnPropertyChanged;
        GlobalState.SavegameLoadChanged += OnGlobalState_SavegameLoadChanged;
        _chunkRegionManagerService.PropertyChanged2 += OnPropertyChanged;

        for (int rx = 0; rx < 1; rx++)
            for (int ry = 0; ry < 1; ry++)
            {
                RegionImageModel regionImageModel = _regionImageModelFactory.Create(new Point2<int>(rx, ry));
                for (int x = 0; x < regionImageModel.Image.Size.Width; x++)
                    for (int y = 0; y < regionImageModel.Image.Size.Height; y++)
                        regionImageModel.Image[x, y] = Random.Shared.NextColor();
                regionImageModel.Image.Redraw();
                _regionImageModels.Add(regionImageModel);
            }
    }

    public GlobalState GlobalState { get; }

    // TODO we can encapsulate these properties bindings into separate class
    public float UnitMultiplier => _zoomTable[_zoomLevel];
    public Point2ZRange<int> VisibleChunkRange => _chunkRegionManagerService.VisibleChunkRange;
    public Point2ZRange<int> VisibleRegionRange => _chunkRegionManagerService.VisibleRegionRange;

    public event Action? ViewportSizeRequested;

    partial void OnScreenSizeChanged(Size<int> value) => UpdateChunkRegionManagerService();
    partial void OnCameraPosChanged(Point2Z<float> value) => UpdateChunkRegionManagerService();
    partial void OnZoomLevelChanged(int value) => UpdateChunkRegionManagerService();

    private void UpdateChunkRegionManagerService()
    {
        _chunkRegionManagerService.Update(CameraPos, UnitMultiplier, ScreenSize);
    }

    private void OnGlobalState_SavegameLoadChanged(SavegameLoadState obj)
    {
        ViewportSizeRequested?.Invoke();
    }

    [RelayCommand]
    private void OnScreenSizeChanged(SizeChangedEventArgs e)
    {
        Size newSize = e.NewSize;
        ScreenSize = new Size<int>(newSize.Width.Floor(), newSize.Height.Floor());
    }

    [RelayCommand]
    private void OnMouseMove(MouseEventArgs e)
    {
        Point point = e.GetPosition(e.Source as IInputElement);
        Point2<int> oldMousePos = MousePos;
        Point2<int> newMousePos = new Point2<int>(point.X.Floor(), point.Y.Floor());
        Vector2<int> newMousePosDelta = newMousePos - oldMousePos;
        MousePos = newMousePos;
        MousePosDelta = MouseInitClickDrag && MouseClickHolding ? Vector2<int>.Zero : newMousePosDelta;
        if (MouseClickHolding)
        {
            Vector2Z<float> cameraPosDelta = new(-MousePosDelta.X / UnitMultiplier, -MousePosDelta.Y / UnitMultiplier);
            CameraPos += cameraPosDelta;
            MouseInitClickDrag = false;
        }
    }

    [RelayCommand]
    private void OnMouseWheel(MouseWheelEventArgs e)
    {
        int newZoomLevel = ZoomLevel;
        if (e.Delta > 0)
            newZoomLevel++;
        else
            newZoomLevel--;
        newZoomLevel = int.Clamp(newZoomLevel, 0, _zoomTable.Length - 1);
        ZoomLevel = newZoomLevel;
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
        MouseIsInside = true;
    }

    [RelayCommand]
    private void OnMouseLeave()
    {
        MouseIsInside = false;
        MouseClickHolding = false;
    }
}
