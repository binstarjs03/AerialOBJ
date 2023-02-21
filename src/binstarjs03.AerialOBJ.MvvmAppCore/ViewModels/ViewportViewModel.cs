using System;
using System.Collections.ObjectModel;

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

    private readonly float[] _zoomTable = new float[] {
        1, 2, 3, 5, 8, 13, 21, 34 // fib. sequence
    };
    private const float s_zoomLowLimit = 1f;
    private const float s_zoomHighLimit = 32f;

    [ObservableProperty] private PointZ<float> _cameraPos = PointZ<float>.Zero;
    [ObservableProperty] private Size<int> _screenSize = new(0, 0);
    [ObservableProperty] private float _zoomMultiplier = 1f;

    [ObservableProperty] private int _heightLevel = 0;
    [ObservableProperty] private int _lowHeightLimit = 0;
    [ObservableProperty] private int _highHeightLimit = 0;

    public ViewportViewModel(GlobalState globalState,
                                     Setting setting,
                                     IChunkRegionManager chunkRegionManager,
                                     ILogService logService,
                                     IModalService modalService,
                                     IMouse mouse)
    {
        _globalState = globalState;
        ChunkRegionManager = chunkRegionManager;
        _logService = logService;
        _modalService = modalService;
        Mouse = mouse;
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
    public ObservableCollection<RegionDataImageModel> RegionDataImageModels { get; } = new();

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

    private void OnSavegameLoadInfoChanged(SavegameLoadState state)
    {
        if (state == SavegameLoadState.Opened)
            InitializeOnSavegameOpened();
        else if (state == SavegameLoadState.Closed)
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
        LowHeightLimit = _globalState.SavegameLoadInfo!.LowHeightLimit;
        HighHeightLimit = _globalState.SavegameLoadInfo!.HighHeightLimit;
        HeightLevel = HighHeightLimit;

        if (ViewportSizeProvider is not null)
            ScreenSize = ViewportSizeProvider.Invoke();

        ChunkRegionManager.StartBackgroundThread();
        ChunkRegionManager.UpdateHeightLevel(HeightLevel);
        UpdateChunkRegionManager();
    }

    private void CleanupOnSavegameClosed()
    {
        ChunkRegionManager.Reinitialize();

        CameraPos = PointZ<float>.Zero;
        ZoomMultiplier = _zoomTable[0];
        ScreenSize = new Size<int>(0, 0);

        LowHeightLimit = 0;
        HighHeightLimit = 0;
        HeightLevel = 0;
    }

    private void UpdateChunkRegionManager()
    {
        InvokeIfSavegameLoaded(() => ChunkRegionManager.Update(CameraPos, ScreenSize, ZoomMultiplier));
    }

    private void UpdateChunkRegionManagerHeight()
    {
        InvokeIfSavegameLoaded(() => ChunkRegionManager.UpdateHeightLevel(HeightLevel));
    }

    [RelayCommand]
    private void ScreenSizeChanged(Size<int> e) => ScreenSize = e;

    partial void OnCameraPosChanged(PointZ<float> value) => UpdateChunkRegionManager();
    partial void OnScreenSizeChanged(Size<int> value) => UpdateChunkRegionManager();
    partial void OnZoomMultiplierChanged(float value) => UpdateChunkRegionManager();
    partial void OnHeightLevelChanged(int value) => UpdateChunkRegionManagerHeight();
}
