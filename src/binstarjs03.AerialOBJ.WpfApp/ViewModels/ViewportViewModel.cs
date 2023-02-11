using System;
using System.Collections.ObjectModel;
using System.Windows;

using binstarjs03.AerialOBJ.Core;
using binstarjs03.AerialOBJ.Core.Definitions;
using binstarjs03.AerialOBJ.Core.MinecraftWorld;
using binstarjs03.AerialOBJ.Core.Primitives;
using binstarjs03.AerialOBJ.WpfApp.Models;
using binstarjs03.AerialOBJ.WpfApp.Models.Settings;
using binstarjs03.AerialOBJ.WpfApp.Services;
using binstarjs03.AerialOBJ.WpfApp.Services.ModalServices;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace binstarjs03.AerialOBJ.WpfApp.ViewModels;

[ObservableObject]
public partial class ViewportViewModel : IViewportViewModel
{
    private readonly ILogService _logService;
    private readonly IModalService _modalService;
    private readonly DefinitionSetting _definitionSetting;

    private readonly float[] _zoomTable = new float[] { 1, 2, 3, 5, 8, 13, 21, 34 }; // fib. sequence
    private readonly float _zoomLowLimit = 1f;
    private readonly float _zoomHighLimit = 32f;

    // viewport UI states
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsRegionTextVisible))]
    private bool _isChunkGridVisible = false;
    [ObservableProperty] private bool _isInfoPanelVisible = false;
    [ObservableProperty] private bool _isCameraPositionVisible = false;

    // viewport states
    [ObservableProperty] private Size<int> _screenSize = new(0, 0);
    [ObservableProperty] private PointZ<float> _cameraPos = PointZ<float>.Zero;
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(UnitMultiplier))]
    [NotifyPropertyChangedFor(nameof(IsRegionTextVisible))]
    [NotifyPropertyChangedFor(nameof(SelectionSize))]
    private float _zoomMultiplier = 1f;

    [ObservableProperty] private int _heightLevel = 0;
    [ObservableProperty] private int _lowHeightLimit = 0;
    [ObservableProperty] private int _highHeightLimit = 0;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SelectionStart))]
    [NotifyPropertyChangedFor(nameof(SelectionSize))]
    private PointZ<int> _selection1;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SelectionStart))]
    [NotifyPropertyChangedFor(nameof(SelectionSize))]
    private PointZ<int> _selection2;

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
                             IModalService modalService,
                             ViewportViewModelInputHandler inputHandler,
                             Setting setting)
    {
        AppInfo = appInfo;
        GlobalState = globalState;
        ChunkRegionManager = chunkRegionManager;
        _logService = logService;
        _modalService = modalService;
        InputHandler = inputHandler;
        InputHandler.Viewport = this;
        _definitionSetting = setting.DefinitionSetting;

        GlobalState.SavegameLoadInfoChanged += OnSavegameLoadInfoChanged;

        setting.DefinitionSetting.ViewportDefinitionChanged += () => ChunkRegionManagerAction(ChunkRegionManager.ReloadRenderedChunks);
        setting.ViewportSetting.ChunkShaderChanged += () => ChunkRegionManagerAction(ChunkRegionManager.ReloadRenderedChunks);
        setting.PerformanceSetting.ViewportChunkThreadsChanged += () => ChunkRegionManagerAction(ChunkRegionManager.StartBackgroundThread);

        ChunkRegionManager.RegionLoaded += regionModel => ChunkRegionManagerAction(_regionDataImageModels.Add, regionModel);
        ChunkRegionManager.RegionUnloaded += regionModel => _regionDataImageModels.Remove(regionModel);
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
    public PointZ<int> SelectionStart => new(Math.Min(Selection1.X, Selection2.X), Math.Min(Selection1.Z, Selection2.Z));
    public Size<int> SelectionSize => new((Math.Abs(Selection2.X - Selection1.X) * ZoomMultiplier).Ceiling(), (Math.Abs(Selection2.Z - Selection1.Z) * ZoomMultiplier).Ceiling());

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
            throw new NotImplementedException();
    }

    /// <summary>
    /// Execute only if savegame loaded
    /// </summary>
    private void ChunkRegionManagerAction(Action callback)
    {
        if (!GlobalState.HasSavegameLoaded)
            return;
        callback();
    }

    /// <summary>
    /// Execute only if savegame loaded
    /// </summary>
    private void ChunkRegionManagerAction<T>(Action<T> callback, T arg)
    {
        if (!GlobalState.HasSavegameLoaded)
            return;
        callback(arg);
    }

    private void OnRegionLoadingException(PointZ<int> regionCoords, Exception e)
    {
        _logService.LogException($"Cannot load region {regionCoords}", e);
        _modalService.ShowErrorMessageBox(new MessageBoxArg
        {
            Caption = "Error Loading Region",
            Message = $"An exception occured while loading region {regionCoords}. "
                    + "See debug log window for exception detail"
        });
    }

    private void OnChunkLoadingException(PointZ<int> chunkCoords, Exception e)
    {
        _logService.LogException($"Cannot load chunk {chunkCoords}", e);
        _modalService.ShowErrorMessageBox(new MessageBoxArg
        {
            Caption = "Error Loading Chunk",
            Message = $"An exception occured while loading chunk {chunkCoords}. "
                    + "See debug log window for exception detail"
        });
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

        Selection1 = PointZ<int>.Zero;
        Selection2 = PointZ<int>.Zero;

        IsChunkGridVisible = false;
        IsInfoPanelVisible = false;
        IsCameraPositionVisible = false;
    }

    private void UpdateChunkRegionManager()
    {
        ChunkRegionManagerAction(() => ChunkRegionManager.Update(CameraPos, UnitMultiplier, ScreenSize));
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
