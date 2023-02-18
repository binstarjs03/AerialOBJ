using System;
using System.Collections.ObjectModel;

using binstarjs03.AerialOBJ.Core.Primitives;
using binstarjs03.AerialOBJ.MVVM.Models;
using binstarjs03.AerialOBJ.MVVM.Services;
using binstarjs03.AerialOBJ.MVVM.Services.ModalServices;

namespace binstarjs03.AerialOBJ.MVVM.ViewModels.Viewport;
public class ViewportRegionImageViewModel
{
    private readonly GlobalState _globalState;
    private readonly ILogService _logService;
    private readonly IModalService _modalService;
    private readonly IChunkRegionManager _chunkRegionManager;
    private readonly IViewportInfo _viewportInfo;

    public ViewportRegionImageViewModel(GlobalState globalState,
                                        ILogService logService,
                                        IModalService modalService,
                                        IChunkRegionManager chunkRegionManager,
                                        IViewportInfo viewportInfo)
    {
        _globalState = globalState;
        _logService = logService;
        _modalService = modalService;
        _chunkRegionManager = chunkRegionManager;
        _viewportInfo = viewportInfo;

        _globalState.SavegameLoadInfoChanged += OnSavegameLoadInfoChanged;

        _chunkRegionManager.RegionLoaded += r => HasSavegameCallback(() => RegionDataImageModels.Add(r));
        _chunkRegionManager.RegionUnloaded += r => RegionDataImageModels.Remove(r);
        _chunkRegionManager.RegionLoadingException += (c, e) => OnLoadingException(c, e, "Region");
        _chunkRegionManager.ChunkLoadingException += (c, e) => OnLoadingException(c, e, "Chunk");

        _viewportInfo.CameraPosChanged += _ => UpdateChunkRegionManager();
        _viewportInfo.ScreenSizeChanged += _ => UpdateChunkRegionManager();
        _viewportInfo.ZoomMultiplierChanged += _ => UpdateChunkRegionManager();
        _viewportInfo.HeightLevelChanged += UpdateChunkRegionManagerHeight;
    }

    public ObservableCollection<RegionDataImageModel> RegionDataImageModels { get; } = new();

    private void OnSavegameLoadInfoChanged(SavegameLoadState state)
    {
        //if (state == SavegameLoadState.Opened)
        //    //InitializeOnSavegameOpened();
        //else if (state == SavegameLoadState.Closed)
        //    CleanupOnSavegameClosed();
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

    private void UpdateChunkRegionManager()
    {
        var cameraPos = _viewportInfo.CameraPos;
        var screenSize = _viewportInfo.ScreenSize;
        var unitMultiplier = _viewportInfo.ZoomMultiplier;
        HasSavegameCallback(() => _chunkRegionManager.Update(cameraPos,
                                                                  screenSize,
                                                                  unitMultiplier));
    }

    private void UpdateChunkRegionManagerHeight(int heightLevel)
    {
        HasSavegameCallback(() => _chunkRegionManager.UpdateHeightLevel(heightLevel));
    }

    private void HasSavegameCallback(Action callback)
    {
        if (!_globalState.HasSavegameLoaded)
            return;
        callback();
    }
}
