using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;

using binstarjs03.AerialOBJ.Core;
using binstarjs03.AerialOBJ.Core.Definitions;
using binstarjs03.AerialOBJ.Core.MinecraftWorldRefactor;
using binstarjs03.AerialOBJ.Core.Primitives;
using binstarjs03.AerialOBJ.WpfApp.Components;
using binstarjs03.AerialOBJ.WpfApp.ExtensionMethods;
using binstarjs03.AerialOBJ.WpfApp.Models;
using binstarjs03.AerialOBJ.WpfApp.Services;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using PointSpaceConversion = binstarjs03.AerialOBJ.Core.MathUtils.PointSpaceConversion;

namespace binstarjs03.AerialOBJ.WpfApp.ViewModels;
[ObservableObject]
public partial class ViewportViewModel : IViewportViewModel
{
    private readonly float[] _zoomTable = new float[] { 1, 2, 3, 5, 8, 13, 21, 34 };
    private readonly IChunkRegionManagerService _chunkRegionManagerService;
    private readonly ILogService _logService;
    private readonly DefinitionManagerService _definitionManager;

    [ObservableProperty] private Size<int> _screenSize = new(0, 0);
    [ObservableProperty] private Point2Z<float> _cameraPos = Point2Z<float>.Zero;
    [ObservableProperty][NotifyPropertyChangedFor(nameof(UnitMultiplier))] private int _zoomLevel = 0;
    [ObservableProperty] private int _heightLevel = 319;

    [ObservableProperty] private Point2<int> _mouseScreenPos = Point2<int>.Zero;
    [ObservableProperty] private Vector2<int> _mousePosDelta = Vector2<int>.Zero;
    [ObservableProperty] private bool _mouseClickHolding = false;
    [ObservableProperty] private bool _mouseInitClickDrag = true;
    [ObservableProperty] private bool _mouseIsInside = false;

    [ObservableProperty] private Point3<int> _mouseBlockCoords = Point3<int>.Zero;
    [ObservableProperty] private Point2Z<int> _mouseChunkCoords = Point2Z<int>.Zero;
    [ObservableProperty] private Point2Z<int> _mouseRegionCoords = Point2Z<int>.Zero;
    [ObservableProperty] private string _mouseBlockName = "";
    [ObservableProperty] private string _mouseBlockDisplayName = "";

    // Region Images
    [ObservableProperty] private ObservableCollection<RegionModel> _regionModels = new();

    public ViewportViewModel(GlobalState globalState, IChunkRegionManagerService chunkRegionManagerService, ILogService logService, DefinitionManagerService definitionManager)
    {
        GlobalState = globalState;
        _chunkRegionManagerService = chunkRegionManagerService;
        _logService = logService;
        _definitionManager = definitionManager;
        GlobalState.PropertyChanged += OnPropertyChanged;
        GlobalState.SavegameLoadChanged += OnGlobalState_SavegameLoadChanged;
        _chunkRegionManagerService.PropertyChanged += OnPropertyChanged;
        _chunkRegionManagerService.RegionLoaded += OnChunkRegionManagerService_RegionImageLoaded;
        _chunkRegionManagerService.RegionUnloaded += OnChunkRegionManagerService_RegionImageUnloaded;
        _chunkRegionManagerService.RegionLoadingError += OnChunkRegionManagerService_RegionReadingError;
    }

    public GlobalState GlobalState { get; }
    public float UnitMultiplier => _zoomTable[_zoomLevel];

    // TODO we can encapsulate these properties bindings into separate class
    public Point2ZRange<int> VisibleRegionRange => _chunkRegionManagerService.VisibleRegionRange;
    public int LoadedRegionsCount => _chunkRegionManagerService.LoadedRegionsCount;
    public int PendingRegionsCount => _chunkRegionManagerService.PendingRegionsCount;
    public Point2Z<int>? WorkedRegion => _chunkRegionManagerService.WorkedRegion;
    public Point2ZRange<int> VisibleChunkRange => _chunkRegionManagerService.VisibleChunkRange;
    public int LoadedChunksCount => _chunkRegionManagerService.LoadedChunksCount;
    public int PendingChunksCount => _chunkRegionManagerService.PendingChunksCount;
    public int WorkedChunksCount => _chunkRegionManagerService.WorkedChunksCount;

    public delegate void SizeSetter(ref Size<int> size);
    public event SizeSetter? SetViewportScreenSizeRequested;

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
            _chunkRegionManagerService.OnSavegameOpened();
            Size<int> screenSize = ScreenSize;
            SetViewportScreenSizeRequested?.Invoke(ref screenSize);
            ScreenSize = screenSize;
            UpdateChunkRegionManagerService();
        }
        else if (state == SavegameLoadState.Closed)
        {
            _chunkRegionManagerService.OnSavegameClosed();
            CameraPos = new Point2Z<float>(0, 0);
            ZoomLevel = 0;
            ScreenSize = new Size<int>(0, 0);
        }
    }

    private void OnChunkRegionManagerService_RegionImageLoaded(RegionModel regionModel)
    {
        if (GlobalState.HasSavegameLoaded)
            _regionModels.Add(regionModel);
    }

    private void OnChunkRegionManagerService_RegionImageUnloaded(RegionModel regionModel)
    {
        _regionModels.Remove(regionModel);
    }

    private void OnChunkRegionManagerService_RegionReadingError(Point2Z<int> regionCoords, Exception e)
    {
        if (e is Core.MinecraftWorld.RegionNoDataException)
            _logService.Log($"Skipped Region {regionCoords}: file contains no data", useSeparator: true);
        else if (e is InvalidDataException)
            _logService.Log($"Skipped Region {regionCoords}: file is corrupted", LogStatus.Warning, useSeparator: true);
        else
        {
            _logService.Log($"Skipped Region {regionCoords}: Unhandled exception occured:", LogStatus.Error);
            _logService.Log(e.ToString(), useSeparator: true);
        }
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
        // check if this is initial mouse click and dragging, we don't want the delta to be very large
        // during initial click & drag, which will cause the viewport to teleporting to somewhere
        (MouseScreenPos, Vector2<int> updatedMousePosDelta) = updateMouseScreenPosAndDelta();
        MousePosDelta = MouseInitClickDrag && MouseClickHolding ? Vector2<int>.Zero : updatedMousePosDelta;
        if (MouseClickHolding)
        {
            Vector2Z<float> cameraPosDelta = new(-MousePosDelta.X / UnitMultiplier, -MousePosDelta.Y / UnitMultiplier);
            CameraPos += cameraPosDelta;
            MouseInitClickDrag = false;
        }
        updateMouseWorldInformation();

        (Point2<int> newMousePos, Vector2<int> newMousePosDelta) updateMouseScreenPosAndDelta()
        {
            Point point = e.GetPosition(e.Source as IInputElement);
            Point2<int> oldMousePos = MouseScreenPos;
            Point2<int> newMousePos = new(point.X.Floor(), point.Y.Floor());
            Vector2<int> newMousePosDelta = newMousePos - oldMousePos;
            return (newMousePos, newMousePosDelta);
        }

        void updateMouseWorldInformation()
        {
            Size<float> floatScreenSize = new(ScreenSize.Width, ScreenSize.Height);
            Point2<float> floatMouseScreenPos = new(MouseScreenPos.X, MouseScreenPos.Y);
            Point2Z<float> mouseWorldPos = PointSpaceConversion.ConvertScreenPosToWorldPos(floatMouseScreenPos, CameraPos, UnitMultiplier, floatScreenSize);
            Point2Z<int> mouseBlockCoords2 = new(MathUtils.Floor(mouseWorldPos.X), MathUtils.Floor(mouseWorldPos.Z));
            Block? block = _chunkRegionManagerService.GetBlock(mouseBlockCoords2);

            MouseChunkCoords = MathUtils.MinecraftCoordsConversion.GetChunkCoordsAbsFromBlockCoordsAbs(mouseBlockCoords2);
            MouseRegionCoords = MathUtils.MinecraftCoordsConversion.GetRegionCoordsFromChunkCoordsAbs(MouseChunkCoords);
            if (block is not null)
            {
                MouseBlockCoords = block.Value.Coords;
                MouseBlockName = block.Value.Name;
                if (_definitionManager.CurrentViewportDefinition.BlockDefinitions.TryGetValue(block.Value.Name, out ViewportBlockDefinition? bd))
                    MouseBlockDisplayName = bd.DisplayName;
                else
                    MouseBlockDisplayName = _definitionManager.CurrentViewportDefinition.MissingBlockDefinition.DisplayName;
            }
            else
            {
                MouseBlockCoords = new Point3<int>(mouseBlockCoords2.X, 0, mouseBlockCoords2.Z);
                MouseBlockName = "";
                MouseBlockDisplayName = "Unknown (Unloaded Chunk)";
            }

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
