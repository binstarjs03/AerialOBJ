using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

using binstarjs03.AerialOBJ.Core;
using binstarjs03.AerialOBJ.Core.MinecraftWorld;
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
    private readonly IChunkRenderService _chunkRenderService;
    private readonly ILogService _logService;

    [ObservableProperty] private Size<int> _screenSize = new(0, 0);
    [ObservableProperty] private Point2Z<float> _cameraPos = Point2Z<float>.Zero;
    [ObservableProperty][NotifyPropertyChangedFor(nameof(UnitMultiplier))] private int _zoomLevel = 0;
    [ObservableProperty] private int _heightLevel = 319;

    [ObservableProperty] private Point2<int> _mousePos = Point2<int>.Zero;
    [ObservableProperty] private Vector2<int> _mousePosDelta = Vector2<int>.Zero;
    [ObservableProperty] private bool _mouseClickHolding = false;
    [ObservableProperty] private bool _mouseInitClickDrag = true;
    [ObservableProperty] private bool _mouseIsInside = false;

    [ObservableProperty] private ObservableCollection<RegionImageModel> _regionImageModels = new();
    private readonly Dictionary<Point2Z<int>, RegionImageModel> _regionImageModelKeys = new();

    public ViewportViewModel(GlobalState globalState, RegionImageModelFactory regionImageModelFactory, IChunkRegionManagerService chunkRegionManagerService, IChunkRenderService chunkRenderService, ILogService logService)
    {
        GlobalState = globalState;
        _regionImageModelFactory = regionImageModelFactory;
        _chunkRegionManagerService = chunkRegionManagerService;
        _chunkRenderService = chunkRenderService;
        _logService = logService;
        
        GlobalState.PropertyChanged += OnPropertyChanged;
        GlobalState.SavegameLoadChanged += OnGlobalState_SavegameLoadChanged;
        _chunkRegionManagerService.PropertyChanged += OnPropertyChanged;
        _chunkRegionManagerService.RegionLoaded += OnChunkRegionManagerService_RegionLoaded;
        _chunkRegionManagerService.RegionUnloaded += OnChunkRegionManagerService_RegionUnloaded;
        _chunkRegionManagerService.RegionReadingError += OnChunkRegionManagerService_RegionReadingError;
    }

    public GlobalState GlobalState { get; }

    public float UnitMultiplier => _zoomTable[_zoomLevel];
    // TODO we can encapsulate these properties bindings into separate class
    public int CachedRegionsCount => _chunkRegionManagerService.CachedRegionsCount;
    public Point2ZRange<int> VisibleRegionRange => _chunkRegionManagerService.VisibleRegionRange;
    public int LoadedRegionsCount => _chunkRegionManagerService.LoadedRegionsCount;
    public int PendingRegionsCount => _chunkRegionManagerService.PendingRegionsCount;
    public Point2Z<int>? WorkedRegion => _chunkRegionManagerService.WorkedRegion;
    public Point2ZRange<int> VisibleChunkRange => _chunkRegionManagerService.VisibleChunkRange;

    public event Action? ViewportSizeRequested;

    // Update CRM Service, callback when these properties updated
    private void UpdateChunkRegionManagerService()
    {
        if (GlobalState.HasSavegameLoaded)
            _chunkRegionManagerService.Update(CameraPos, UnitMultiplier, ScreenSize);
    }

    partial void OnScreenSizeChanged(Size<int> value) => UpdateChunkRegionManagerService();
    partial void OnCameraPosChanged(Point2Z<float> value) => UpdateChunkRegionManagerService();
    partial void OnZoomLevelChanged(int value) => UpdateChunkRegionManagerService();

    private void OnGlobalState_SavegameLoadChanged(SavegameLoadState state)
    {
        if (state == SavegameLoadState.Opened)
        {
            ViewportSizeRequested?.Invoke();
            UpdateChunkRegionManagerService();
        }
        else if (state == SavegameLoadState.Closed)
        {
            _chunkRegionManagerService.Reinitialize();
            ScreenSize = new Size<int>(0, 0);
        }
    }

    private void OnChunkRegionManagerService_RegionLoaded(Region region)
    {
        RegionImageModel rim = _regionImageModelFactory.Create(region.RegionCoords);
        _chunkRenderService.RenderRandomNoise(rim.Image,
                                              new Color() { Alpha = 255, Red = 64, Green = 128, Blue = 192 },
                                              64);
        _regionImageModelKeys.Add(region.RegionCoords, rim);
        App.Current.Dispatcher.BeginInvoke(() =>
        {
            rim.Image.Redraw();
            _regionImageModels.Add(rim);
        }, DispatcherPriority.Render);
    }

    private void OnChunkRegionManagerService_RegionUnloaded(Region region)
    {
        RegionImageModel rim = _regionImageModelKeys[region.RegionCoords];
        _regionImageModelKeys.Remove(region.RegionCoords);
        App.Current.Dispatcher.BeginInvoke(() => _regionImageModels.Remove(rim), DispatcherPriority.Render);
    }

    private void OnChunkRegionManagerService_RegionReadingError(Point2Z<int> regionCoords, Exception e)
    {
        App.Current.Dispatcher.BeginInvoke(() =>
        {
            // TODO pass in an enum of error type for more readability
            if (e is RegionNoDataException)
                _logService.Log($"Skipped Region {regionCoords}: file contains no data", useSeparator: true);
            else if (e is InvalidDataException)
                _logService.Log($"Skipped Region {regionCoords}: file is corrupted", LogStatus.Warning, useSeparator: true);
            else
            {
                _logService.Log($"Skipped Region {regionCoords}: Unhandled exception occured:", LogStatus.Error);
                _logService.Log(e.ToString(), useSeparator: true);
            }
        }, DispatcherPriority.Background);
    }

    #region Commands

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

    #endregion Commands
}
