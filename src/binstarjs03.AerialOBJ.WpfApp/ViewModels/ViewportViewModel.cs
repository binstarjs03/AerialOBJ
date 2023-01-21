using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

using binstarjs03.AerialOBJ.Core;
using binstarjs03.AerialOBJ.Core.Definitions;
using binstarjs03.AerialOBJ.Core.MinecraftWorld;
using binstarjs03.AerialOBJ.Core.Primitives;
using binstarjs03.AerialOBJ.WpfApp.Components;
using binstarjs03.AerialOBJ.WpfApp.ExtensionMethods;
using binstarjs03.AerialOBJ.WpfApp.Models;
using binstarjs03.AerialOBJ.WpfApp.Services;
using binstarjs03.AerialOBJ.WpfApp.Services.ChunkRegionManaging;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace binstarjs03.AerialOBJ.WpfApp.ViewModels;

[ObservableObject]
public partial class ViewportViewModel
{
    private readonly ILogService _logService;
    private readonly IDefinitionManager _definitionManager;

    private readonly float[] _zoomTable = new float[] { 1, 2, 3, 5, 8, 13, 21, 34 }; // fib. sequence
    private readonly float _zoomLowLimit = 1f;
    private readonly float _zoomHighLimit = 32f;

    // viewport UI states
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsRegionTextVisible))]
    private bool _isChunkGridVisible = false;
    [ObservableProperty] private bool _isInfoPanelVisible = false;

    // viewport states
    [ObservableProperty] private Size<int> _screenSize = new(0, 0);
    [ObservableProperty] private PointZ<float> _cameraPos = PointZ<float>.Zero;
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(UnitMultiplier))]
    [NotifyPropertyChangedFor(nameof(IsRegionTextVisible))]
    private float _zoomMultiplier = 1f;

    [ObservableProperty] private int _heightLevel = 0;
    [ObservableProperty] private int _lowHeightLimit = 0;
    [ObservableProperty] private int _highHeightLimit = 0;

    // context world states
    [ObservableProperty] private Point3<int> _contextBlockCoords = Point3<int>.Zero;
    [ObservableProperty] private PointZ<int> _contextChunkCoords = PointZ<int>.Zero;
    [ObservableProperty] private PointZ<int> _contextRegionCoords = PointZ<int>.Zero;
    [ObservableProperty] private string _contextBlockName = "";
    [ObservableProperty] private string _contextBlockDisplayName = "";

    // Region Images
    [ObservableProperty] private ObservableCollection<RegionDataImageModel> _regionDataImageModels = new();

    public ViewportViewModel(GlobalState globalState,
                             IChunkRegionManager chunkRegionManager,
                             ILogService logService,
                             IDefinitionManager definitionManager)
    {
        GlobalState = globalState;
        ChunkRegionManager = chunkRegionManager;
        _definitionManager = definitionManager;
        _logService = logService;

        GlobalState.SavegameLoadInfoChanged += OnGlobalState_SavegameLoadInfoChanged;
        ChunkRegionManager.RegionLoaded += ShowRegionDataImageModel;
        ChunkRegionManager.RegionUnloaded += RemoveRegionDataImageModel;
        ChunkRegionManager.RegionLoadingException += OnRegionLoadingException;
        ChunkRegionManager.ChunkLoadingException += OnChunkLoadingException;
        _definitionManager.ViewportDefinitionChanging += OnDefinitionManager_ViewportDefinitionChanging;
        _definitionManager.ViewportDefinitionChanged += OnDefinitionManager_ViewportDefinitionChanged;

        MouseReceiver = new MouseReceiver();
        MouseReceiver.MouseMove += MouseReceiver_MouseMove;
        MouseReceiver.MouseWheel += MouseReceiver_MouseWheel;
    }

    public GlobalState GlobalState { get; }
    public Func<Size<int>>? GetViewViewportSize { get; set; }
    public MouseReceiver MouseReceiver { get; } // TODO abstract this into interface
    public IChunkRegionManager ChunkRegionManager { get; }

    public float UnitMultiplier => ZoomMultiplier;
    public bool IsRegionTextVisible => ZoomMultiplier <= 1f && IsChunkGridVisible;

    #region Event Handlers

    partial void OnScreenSizeChanged(Size<int> value) => UpdateChunkRegionManager();
    partial void OnCameraPosChanged(PointZ<float> value) => UpdateChunkRegionManager();
    partial void OnZoomMultiplierChanged(float value) => UpdateChunkRegionManager();
    partial void OnHeightLevelChanged(int value) => ChunkRegionManager.UpdateHeightLevel(HeightLevel);

    private void OnGlobalState_SavegameLoadInfoChanged(SavegameLoadState state)
    {
        if (state == SavegameLoadState.Opened)
            InitializeOnSavegameOpened();
        else if (state == SavegameLoadState.Closed)
            CleanupOnSavegameClosed();
        else
            throw new NotImplementedException($"No handler implemented for {nameof(SavegameLoadState)} of {state}");
    }

    private void ShowRegionDataImageModel(RegionDataImageModel regionModel)
    {
        if (GlobalState.HasSavegameLoaded)
            _regionDataImageModels.Add(regionModel);
    }

    private void RemoveRegionDataImageModel(RegionDataImageModel regionModel)
    {
        _regionDataImageModels.Remove(regionModel);
    }

    private void OnRegionLoadingException(PointZ<int> regionCoords, Exception e)
    {
        _logService.LogException($"Cannot load region {regionCoords}", e);
    }

    private void OnChunkLoadingException(PointZ<int> chunkCoords, Exception e)
    {
        _logService.LogException($"Cannot load chunk {chunkCoords}", e);
    }

    private void OnDefinitionManager_ViewportDefinitionChanging()
    {
        // We want to request for crm to stop so we can safely swap definition.
        if (!GlobalState.HasSavegameLoaded)
            return;
        ChunkRegionManager.StopBackgroundThread();
    }

    private void OnDefinitionManager_ViewportDefinitionChanged()
    {
        // continue working for crm
        if (!GlobalState.HasSavegameLoaded)
            return;
        ChunkRegionManager.StartBackgroundThread();
        UpdateChunkRegionManager();
    }

    #endregion Event Handlers

    private void InitializeOnSavegameOpened()
    {
        LowHeightLimit = GlobalState.SavegameLoadInfo!.LowHeightLimit;
        HighHeightLimit = GlobalState.SavegameLoadInfo!.HighHeightLimit;
        HeightLevel = HighHeightLimit;

        if (GetViewViewportSize is not null)
            ScreenSize = GetViewViewportSize();

        ChunkRegionManager.StartBackgroundThread();
        ChunkRegionManager.UpdateHeightLevel(HeightLevel);
        UpdateChunkRegionManager();

        if (GlobalState.IsDebugEnabled)
        {
            IsChunkGridVisible = true;
            IsInfoPanelVisible = true;
        }
    }

    private void CleanupOnSavegameClosed()
    {
        ChunkRegionManager.Reinitialize();
        CameraPos = new PointZ<float>(0, 0);
        ZoomMultiplier = _zoomTable[0];
        ScreenSize = new Size<int>(0, 0);
        LowHeightLimit = 0;
        HighHeightLimit = 0;
        HeightLevel = 0;
        IsChunkGridVisible = false;
        IsInfoPanelVisible = false;
    }

    private void UpdateChunkRegionManager()
    {
        if (GlobalState.HasSavegameLoaded)
            ChunkRegionManager.Update(CameraPos, UnitMultiplier, ScreenSize);
    }

    #region Commands

    [RelayCommand]
    private void OnScreenSizeChanged(SizeChangedEventArgs e)
    {
        Size newSize = e.NewSize;
        ScreenSize = new Size<int>(newSize.Width.Floor(), newSize.Height.Floor());
    }

    private void MouseReceiver_MouseMove(PointY<int> mousePos, PointY<int> mouseDelta)
    {
        if (MouseReceiver.IsMouseLeft)
        {
            PointZ<float> cameraPosDelta = -(mouseDelta.ToFloat() / UnitMultiplier);
            TranslateCamera(cameraPosDelta);
        }

        PointZ<float> mouseWorldPos = PointSpaceConversion.ConvertScreenPosToWorldPos(
            mousePos.ToFloat(),
            CameraPos,
            UnitMultiplier,
            ScreenSize.ToFloat());
        UpdateContextWorldInformation(mouseWorldPos.Floor());
    }

    private void MouseReceiver_MouseWheel(int delta)
    {
        if (delta > 0)
            Zoom(ZoomDirection.In);
        else
            Zoom(ZoomDirection.Out);
    }

    private void UpdateContextWorldInformation(PointZ<int> worldPos)
    {
        ContextChunkCoords = MinecraftWorldMathUtils.GetChunkCoordsAbsFromBlockCoordsAbs(worldPos);
        ContextRegionCoords = MinecraftWorldMathUtils.GetRegionCoordsFromChunkCoordsAbs(ContextChunkCoords);
        BlockSlim? block = ChunkRegionManager.GetHighestBlockAt(worldPos);
        if (block is not null)
        {
            ContextBlockCoords = new Point3<int>(worldPos.X, block.Value.Height, worldPos.Z);
            ContextBlockName = block.Value.Name;
            if (_definitionManager.CurrentViewportDefinition.BlockDefinitions.TryGetValue(block.Value.Name, out ViewportBlockDefinition? bd))
                ContextBlockDisplayName = bd.DisplayName;
            else
                ContextBlockDisplayName = _definitionManager.CurrentViewportDefinition.MissingBlockDefinition.DisplayName;
        }
        else
        {
            ContextBlockCoords = new Point3<int>(worldPos.X, 0, worldPos.Z);
            ContextBlockName = "";
            ContextBlockDisplayName = "Unknown (Unloaded Chunk)";
        }
    }

    [RelayCommand]
    private void OnKeyDown(KeyEventArgs e)
    {
        if (e.Key == Key.Up || e.Key == Key.Down || e.Key == Key.Left || e.Key == Key.Right)
            handleDirectionalKey();
        else if (e.Key == Key.Add || e.Key == Key.Subtract)
            handleZoomKey();
        else
            return;

        void handleDirectionalKey()
        {
            PointY<int> direction = new(0, 0);
            if (e.Key == Key.Up)
                direction.Y -= 1;
            if (e.Key == Key.Down)
                direction.Y += 1;
            if (e.Key == Key.Left)
                direction.X -= 1;
            if (e.Key == Key.Right)
                direction.X += 1;
            TranslateCamera(direction.ToFloat() / UnitMultiplier * 200);
            e.Handled = true;
        }

        void handleZoomKey()
        {
            if (e.Key == Key.Add)
                Zoom(ZoomDirection.In);
            else
                Zoom(ZoomDirection.Out);
            e.Handled = true;
        }
    }

    private void TranslateCamera(PointZ<float> displacement)
    {
        CameraPos += displacement;
    }

    private void Zoom(ZoomDirection direction)
    {
        int nearestIndex = 0;

        // snaps to nearest table, find the index
        for (int i = 0; i < _zoomTable.Length; i++)
        {
            if (_zoomTable[i] <= ZoomMultiplier)
                nearestIndex = i;
            else
                // zoom is bigger than table index value, we got the nearest value
                // to snap onto the table (which is the value on last index)
                break;
        }

        // modify the index based on zooming direction
        if (direction == ZoomDirection.In)
            nearestIndex++;
        else
            nearestIndex--;

        // avoid index over/underflow before assignment, and clamp zoom to limit
        nearestIndex = int.Clamp(nearestIndex, 0, _zoomTable.Length - 1);
        float newZoomMultiplier = float.Clamp(_zoomTable[nearestIndex], _zoomLowLimit, _zoomHighLimit);
        ZoomMultiplier = newZoomMultiplier;
    }

    private enum ZoomDirection
    {
        In,
        Out,
    }

    #endregion Commands
}
