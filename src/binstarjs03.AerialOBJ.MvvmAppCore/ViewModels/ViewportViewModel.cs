using System;
using System.Collections.ObjectModel;

using binstarjs03.AerialOBJ.Core.Definitions;
using binstarjs03.AerialOBJ.Core.MinecraftWorld;
using binstarjs03.AerialOBJ.Core.Primitives;
using binstarjs03.AerialOBJ.MvvmAppCore.Models;
using binstarjs03.AerialOBJ.MvvmAppCore.Models.Settings;
using binstarjs03.AerialOBJ.MvvmAppCore.Services;
using binstarjs03.AerialOBJ.MvvmAppCore.Services.Input;
using binstarjs03.AerialOBJ.MvvmAppCore.Services.ModalServices;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace binstarjs03.AerialOBJ.MvvmAppCore.ViewModels;
public partial class ViewportViewModel : ObservableObject
{
    private readonly GlobalState _globalState;
    private readonly ILogService _logService;
    private readonly IModalService _modalService;
    private readonly DefinitionSetting _definitionSetting;

    private readonly float[] _zoomTable = new float[] {
        1, 2, 3, 5, 8, 13, 21, 34 // fib. sequence
    };
    private const float s_zoomLowLimit = 1f;
    private const float s_zoomHighLimit = 32f;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsOriginLineVisible))]
    private bool _isChunkGridVisible = false;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsOriginLineVisible))]
    private bool _isRegionGridVisible = false;

    [ObservableProperty] private bool _isInfoPanelVisible = false;
    [ObservableProperty] private bool _isCameraPositionVisible = false;
    [ObservableProperty] private bool _isCoordinateVisible = false;

    [ObservableProperty] private PointZ<float> _cameraPos = PointZ<float>.Zero;
    [ObservableProperty] private Size<int> _screenSize = new(0, 0);
    [ObservableProperty] private float _zoomMultiplier = 1f;

    [ObservableProperty] private int _heightLevel = 0;
    [ObservableProperty] private int _minHeightLimit = 0;
    [ObservableProperty] private int _maxHeightLimit = 0;

    // context world states
    [ObservableProperty] private Point3<int> _contextBlockCoords = Point3<int>.Zero;
    [ObservableProperty] private PointZ<int> _contextChunkCoords = PointZ<int>.Zero;
    [ObservableProperty] private PointZ<int> _contextRegionCoords = PointZ<int>.Zero;
    [ObservableProperty] private string _contextBlockName = "";
    [ObservableProperty] private string _contextBlockDisplayName = "";

    public ViewportViewModel(GlobalState globalState,
                             Setting setting,
                             IChunkRegionManager chunkRegionManager,
                             ILogService logService,
                             IModalService modalService,
                             IMouse mouse,
                             IKeyboard keyboard)
    {
        _globalState = globalState;
        _definitionSetting = setting.DefinitionSetting;
        ChunkRegionManager = chunkRegionManager;
        _logService = logService;
        _modalService = modalService;
        Mouse = mouse;
        Keyboard = keyboard;
        ViewportCoordsManager = new ViewportCoordsManager();

        globalState.SavegameLoadInfoChanged += OnSavegameLoadInfoChanged;

        setting.DefinitionSetting.ViewportDefinitionChanged += () => InvokeIfSavegameLoaded(chunkRegionManager.ReloadRenderedChunks);
        setting.ViewportSetting.ChunkShaderChanged += () => InvokeIfSavegameLoaded(chunkRegionManager.ReloadRenderedChunks);
        setting.PerformanceSetting.ViewportChunkThreadsChanged += () => InvokeIfSavegameLoaded(chunkRegionManager.StartBackgroundThread);

        chunkRegionManager.RegionLoaded += r => InvokeIfSavegameLoaded(() => RegionDataImageModels.Add(r));
        chunkRegionManager.RegionUnloaded += r => RegionDataImageModels.Remove(r);
        chunkRegionManager.RegionLoadingException += (c, e) => OnLoadingException(c, e, "Region");
        chunkRegionManager.ChunkLoadingException += (c, e) => OnLoadingException(c, e, "Chunk");
    }

    public Func<Size<int>>? ViewportSizeProvider { get; set; }
    public IMouse Mouse { get; }
    public IKeyboard Keyboard { get; }
    public ViewportCoordsManager ViewportCoordsManager { get; }
    public ObservableCollection<RegionDataImageModel> RegionDataImageModels { get; } = new();
    public bool IsOriginLineVisible => IsChunkGridVisible || IsRegionGridVisible;

    public IChunkRegionManager ChunkRegionManager { get; }

    public void Zoom(ZoomDirection direction)
    {
        int nearestIndex = 0;

        // snaps current zoom multiplier to nearest table value
        for (int i = 0; i < _zoomTable.Length; i++)
        {
            if (_zoomTable[i] <= ZoomMultiplier)
                nearestIndex = i;
            else
                break;
        }

        // modify the index based on zooming direction. out direction handling is a bit
        // different, we are already zooming out from snapping, zoom out if equal to snap
        if (direction == ZoomDirection.In)
            nearestIndex++;
        else if (direction == ZoomDirection.Out)
            if (ZoomMultiplier == _zoomTable[nearestIndex])
                nearestIndex--;

        // clamp index over/underflow from modifying
        nearestIndex = int.Clamp(nearestIndex, 0, _zoomTable.Length - 1);
        float newZoomMultiplier = float.Clamp(_zoomTable[nearestIndex], s_zoomLowLimit, s_zoomHighLimit);
        ZoomMultiplier = newZoomMultiplier;
    }

    public void TranslateCamera(PointZ<float> displacement) => CameraPos += displacement;

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

    public void MoveHeightLevel(HeightLevelDirection direction, int distance)
    {
        int difference = direction == HeightLevelDirection.Up? 1 : -1;
        int newHeightLevel = HeightLevel + distance * difference;
        newHeightLevel = Math.Clamp(newHeightLevel, MinHeightLimit, MaxHeightLimit);
        HeightLevel = newHeightLevel;
    }

    private void OnSavegameLoadInfoChanged(SavegameLoadInfo? info)
    {
        if (info is not null)
            InitializeOnSavegameOpened();
        else
            CleanupOnSavegameClosed();
    }

    private void InvokeIfSavegameLoaded(Action callback)
    {
        if (_globalState.HasSavegameLoaded)
            callback.Invoke();
    }

    private void OnLoadingException(PointZ<int> coords, Exception e, string loadWhat)
    {
        _logService.LogException($"Cannot load {loadWhat} {coords}", e);
        _modalService.ShowErrorMessageBox(new MessageBoxArg
        {
            Caption = $"Error Loading {loadWhat}",
            Message = $"An exception occured while loading {loadWhat} {coords}. "
                    + "See debug log window for exception detail"
        });
    }

    private void InitializeOnSavegameOpened()
    {
        MinHeightLimit = _globalState.SavegameLoadInfo!.LowHeightLimit;
        MaxHeightLimit = _globalState.SavegameLoadInfo!.HighHeightLimit;
        HeightLevel = MaxHeightLimit;

        if (ViewportSizeProvider is not null)
            ScreenSize = ViewportSizeProvider.Invoke();

        ChunkRegionManager.StartBackgroundThread();
        ChunkRegionManager.UpdateHeightLevel(HeightLevel);
        UpdateChunkRegionManager();
        UpdateViewportCoordsManager();
    }

    private void CleanupOnSavegameClosed()
    {
        ChunkRegionManager.Reinitialize();
        ViewportCoordsManager.Reinitialize();

        CameraPos = PointZ<float>.Zero;
        ZoomMultiplier = _zoomTable[0];
        ScreenSize = new Size<int>(0, 0);

        MinHeightLimit = 0;
        MaxHeightLimit = 0;
        HeightLevel = 0;

        IsChunkGridVisible = false;
        IsRegionGridVisible = false;

        IsInfoPanelVisible = false;
        IsCameraPositionVisible = false;
        IsCoordinateVisible = false;
    }

    private void UpdateChunkRegionManager()
    {
        InvokeIfSavegameLoaded(() => ChunkRegionManager.Update(CameraPos, ScreenSize, ZoomMultiplier));
    }

    private void UpdateViewportCoordsManager()
    {
        InvokeIfSavegameLoaded(() =>
        {
            if (IsCoordinateVisible)
                ViewportCoordsManager.Update(IRegion.BlockCount / 2, CameraPos, ScreenSize, ZoomMultiplier);
        });
    }

    private void UpdateChunkRegionManagerHeight()
    {
        InvokeIfSavegameLoaded(() => ChunkRegionManager.UpdateHeightLevel(HeightLevel));
    }

    [RelayCommand]
    private void ScreenSizeChanged(Size<int> e) => ScreenSize = e;

    partial void OnCameraPosChanged(PointZ<float> value)
    {
        UpdateChunkRegionManager();
        UpdateViewportCoordsManager();
    }

    partial void OnScreenSizeChanged(Size<int> value)
    {
        UpdateChunkRegionManager();
        UpdateViewportCoordsManager();
    }

    partial void OnZoomMultiplierChanged(float value)
    {
        ViewportCoordsManager.Reinitialize();
        UpdateChunkRegionManager();
        UpdateViewportCoordsManager();
    }

    partial void OnHeightLevelChanged(int value) => UpdateChunkRegionManagerHeight();

    partial void OnIsCoordinateVisibleChanged(bool value)
    {
        if (!value)
            ViewportCoordsManager.Reinitialize();
        else
            UpdateViewportCoordsManager();
    }
}