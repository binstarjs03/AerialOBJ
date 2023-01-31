using System;
using System.Collections.ObjectModel;
using System.Windows;

using binstarjs03.AerialOBJ.Core;
using binstarjs03.AerialOBJ.Core.Definitions;
using binstarjs03.AerialOBJ.Core.MinecraftWorld;
using binstarjs03.AerialOBJ.Core.Primitives;
using binstarjs03.AerialOBJ.WpfApp.Models;
using binstarjs03.AerialOBJ.WpfApp.Services;
using binstarjs03.AerialOBJ.WpfApp.Services.ChunkRegionManaging;
using binstarjs03.AerialOBJ.WpfApp.Settings;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace binstarjs03.AerialOBJ.WpfApp.ViewModels;

[ObservableObject]
public partial class ViewportViewModel : IViewportViewModel
{
    private readonly ILogService _logService;
    private readonly DefinitionSetting _definitionSetting;

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

    public ViewportViewModel(AppInfo appInfo,
                             GlobalState globalState,
                             IChunkRegionManager chunkRegionManager,
                             ILogService logService,
                             ViewportViewModelInputHandler inputHandler,
                             Setting setting)
    {
        AppInfo = appInfo;
        GlobalState = globalState;
        ChunkRegionManager = chunkRegionManager;
        _logService = logService;
        InputHandler = inputHandler;
        _definitionSetting = setting.DefinitionSetting;

        InputHandler.Viewport = this;
        GlobalState.SavegameLoadInfoChanged += OnSavegameLoadInfoChanged;
        _definitionSetting.ViewportDefinitionChanging += OnViewportDefinitionChanging;
        _definitionSetting.ViewportDefinitionChanged += OnViewportDefinitionChanged;
        ChunkRegionManager.RegionLoaded += ShowRegionImage;
        ChunkRegionManager.RegionUnloaded += RemoveRegionImage;
        ChunkRegionManager.RegionLoadingException += OnRegionLoadingException;
        ChunkRegionManager.ChunkLoadingException += OnChunkLoadingException;
    }

    public AppInfo AppInfo { get; }
    public GlobalState GlobalState { get; }
    public Func<Size<int>>? GetViewViewportSize { get; set; }
    public IChunkRegionManager ChunkRegionManager { get; }
    public ViewportViewModelInputHandler InputHandler { get; }

    public float UnitMultiplier => ZoomMultiplier;
    public bool IsRegionTextVisible => ZoomMultiplier <= 1f && IsChunkGridVisible;

    partial void OnScreenSizeChanged(Size<int> value) => UpdateChunkRegionManager();
    partial void OnCameraPosChanged(PointZ<float> value) => UpdateChunkRegionManager();
    partial void OnZoomMultiplierChanged(float value) => UpdateChunkRegionManager();
    partial void OnHeightLevelChanged(int value) => ChunkRegionManager.UpdateHeightLevel(HeightLevel);

    private void OnSavegameLoadInfoChanged(SavegameLoadState state)
    {
        if (state == SavegameLoadState.Opened)
            InitializeOnSavegameOpened();
        else if (state == SavegameLoadState.Closed)
            CleanupOnSavegameClosed();
        else
            throw new NotImplementedException($"No handler implemented for {nameof(SavegameLoadState)} of {state}");
    }

    private void OnViewportDefinitionChanging()
    {
        // We want to request for crm to stop so we can safely swap definition.
        if (!GlobalState.HasSavegameLoaded)
            return;
        ChunkRegionManager.StopBackgroundThread();
    }

    private void OnViewportDefinitionChanged()
    {
        // continue working for crm
        if (!GlobalState.HasSavegameLoaded)
            return;
        ChunkRegionManager.StartBackgroundThread();
        UpdateChunkRegionManager();
    }

    private void ShowRegionImage(RegionDataImageModel regionModel)
    {
        if (GlobalState.HasSavegameLoaded)
            _regionDataImageModels.Add(regionModel);
    }

    private void RemoveRegionImage(RegionDataImageModel regionModel)
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

        if (AppInfo.IsDebugEnabled)
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

    [RelayCommand]
    private void OnScreenSizeChanged(SizeChangedEventArgs e)
    {
        Size newSize = e.NewSize;
        ScreenSize = new Size<int>(newSize.Width.Floor(), newSize.Height.Floor());
    }

    public void UpdateContextWorldInformation(PointZ<int> worldPos)
    {
        ContextChunkCoords = MinecraftWorldMathUtils.GetChunkCoordsAbsFromBlockCoordsAbs(worldPos);
        ContextRegionCoords = MinecraftWorldMathUtils.GetRegionCoordsFromChunkCoordsAbs(ContextChunkCoords);
        BlockSlim? block = ChunkRegionManager.GetHighestBlockAt(worldPos);
        if (block is not null)
        {
            ContextBlockCoords = new Point3<int>(worldPos.X, block.Value.Height, worldPos.Z);
            ContextBlockName = block.Value.Name;
            if (_definitionSetting.CurrentViewportDefinition.BlockDefinitions.TryGetValue(block.Value.Name, out ViewportBlockDefinition? bd))
                ContextBlockDisplayName = bd.DisplayName;
            else
                ContextBlockDisplayName = _definitionSetting.CurrentViewportDefinition.MissingBlockDefinition.DisplayName;
        }
        else
        {
            ContextBlockCoords = new Point3<int>(worldPos.X, 0, worldPos.Z);
            ContextBlockName = "";
            ContextBlockDisplayName = "Unknown (Unloaded Chunk)";
        }
    }
    public void TranslateCamera(PointZ<float> displacement)
    {
        CameraPos += displacement;
    }

    public void Zoom(ZoomDirection direction)
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

        // modify the index based on zooming direction. out direction handling is a bit
        // different, we are already zooming out from snapping, zoom out if equal to snap
        if (direction == ZoomDirection.In)
            nearestIndex++;
        else if (direction == ZoomDirection.Out)
            if (ZoomMultiplier == _zoomTable[nearestIndex])
                nearestIndex--;

        // avoid index over/underflow before assignment
        nearestIndex = int.Clamp(nearestIndex, 0, _zoomTable.Length - 1);
        float newZoomMultiplier = float.Clamp(_zoomTable[nearestIndex], _zoomLowLimit, _zoomHighLimit);
        ZoomMultiplier = newZoomMultiplier;
    }

    public void MoveHeightLevel(HeightLevelDirection direction, int distance)
    {
        int difference = direction == HeightLevelDirection.Up ? 1 : -1;
        int newHeightLevel = HeightLevel + distance * difference;
        newHeightLevel = Math.Clamp(newHeightLevel, LowHeightLimit, HighHeightLimit);
        HeightLevel = newHeightLevel;
    }
}
