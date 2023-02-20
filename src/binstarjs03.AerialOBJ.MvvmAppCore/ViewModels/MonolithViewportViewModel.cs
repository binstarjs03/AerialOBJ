using System;
using System.Collections.ObjectModel;

using binstarjs03.AerialOBJ.Core.Primitives;
using binstarjs03.AerialOBJ.MvvmAppCore.Models;
using binstarjs03.AerialOBJ.MvvmAppCore.Models.Settings;
using binstarjs03.AerialOBJ.MvvmAppCore.Services;
using binstarjs03.AerialOBJ.MvvmAppCore.Services.ModalServices;

using CommunityToolkit.Mvvm.ComponentModel;

namespace binstarjs03.AerialOBJ.MvvmAppCore.ViewModels;
public partial class MonolithViewportViewModel : ObservableObject
{
    private readonly GlobalState _globalState;
    private readonly IChunkRegionManager _chunkRegionManager;
    private readonly ILogService _logService;
    private readonly IModalService _modalService;

    private readonly float[] _zoomTable = new float[] {
        1, 2, 3, 5, 8, 13, 21, 34 // fib. sequence
    }; 
    //private const float s_zoomLowLimit = 1f;
    //private const float s_zoomHighLimit = 32f;

    [ObservableProperty] private PointZ<float> _cameraPos = PointZ<float>.Zero;
    [ObservableProperty] private Size<int> _screenSize = new(0,0);
    [ObservableProperty] private float _zoomMultiplier = 1f;

    [ObservableProperty] private int _heightLevel = 0;
    [ObservableProperty] private int _lowHeightLimit = 0;
    [ObservableProperty] private int _highHeightLimit = 0;

    public MonolithViewportViewModel(GlobalState globalState,
                                     Setting setting,
                                     IChunkRegionManager chunkRegionManager,
                                     ILogService logService,
                                     IModalService modalService)
    {
        _globalState = globalState;
        _chunkRegionManager = chunkRegionManager;
        _logService = logService;
        _modalService = modalService;

        globalState.SavegameLoadInfoChanged += OnSavegameLoadInfoChanged;

        setting.DefinitionSetting.ViewportDefinitionChanged += () => InvokeIfSavegameLoaded(chunkRegionManager.ReloadRenderedChunks);
        setting.ViewportSetting.ChunkShaderChanged += () => InvokeIfSavegameLoaded(chunkRegionManager.ReloadRenderedChunks);
        setting.PerformanceSetting.ViewportChunkThreadsChanged += () => InvokeIfSavegameLoaded(_chunkRegionManager.StartBackgroundThread);

        chunkRegionManager.RegionLoaded += r => InvokeIfSavegameLoaded(() => RegionDataImageModels.Add(r));
        chunkRegionManager.RegionUnloaded += r => RegionDataImageModels.Remove(r);
        chunkRegionManager.RegionLoadingException += (c, e) => OnLoadingException(c, e, "Region");
        chunkRegionManager.ChunkLoadingException += (c, e) => OnLoadingException(c, e, "Chunk");
    }

    public Func<Size<int>>? ViewportSizeProvider { get; set; }
    public ObservableCollection<RegionDataImageModel> RegionDataImageModels { get; } = new();

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

        _chunkRegionManager.StartBackgroundThread();
        _chunkRegionManager.UpdateHeightLevel(HeightLevel);
        UpdateChunkRegionManager();
    }

    private void CleanupOnSavegameClosed()
    {
        _chunkRegionManager.Reinitialize();

        CameraPos = PointZ<float>.Zero;
        ZoomMultiplier = _zoomTable[0];
        ScreenSize = new Size<int>(0,0);

        LowHeightLimit = 0;
        HighHeightLimit = 0;
        HeightLevel= 0;
    }

    private void UpdateChunkRegionManager()
    {
        InvokeIfSavegameLoaded(() => _chunkRegionManager.Update(CameraPos, ScreenSize, ZoomMultiplier));
    }

    //private void OnScreenSizeChanged()
}
