using binstarjs03.AerialOBJ.Core;
using binstarjs03.AerialOBJ.Core.MinecraftWorld;
using binstarjs03.AerialOBJ.Core.Primitives;
using binstarjs03.AerialOBJ.MvvmAppCore.ViewTraits;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace binstarjs03.AerialOBJ.MvvmAppCore.ViewModels;
public partial class GotoViewModel : ObservableObject
{
    private readonly GlobalState _globalState;
    private readonly IViewportViewModel _viewportViewModel;

    private bool _isActive = false;

    [ObservableProperty] private int _blockCoordsX = 0;
    [ObservableProperty] private int _blockCoordsY = 0;
    [ObservableProperty] private int _blockCoordsZ = 0;

    [ObservableProperty] private int _chunkCoordsX = 0;
    [ObservableProperty] private int _chunkCoordsY = 0;
    [ObservableProperty] private int _chunkCoordsZ = 0;

    [ObservableProperty] private int _regionCoordsX = 0;
    [ObservableProperty] private int _regionCoordsZ = 0;

    public GotoViewModel(GlobalState globalState, IViewportViewModel viewportViewModel)
    {
        _globalState = globalState;
        _viewportViewModel = viewportViewModel;

        globalState.SavegameLoadInfoChanged += OnSavegameLoadInfoChanged;
        viewportViewModel.CameraPosChanged += OnViewportCameraPosChanged;
        viewportViewModel.HeightLevelChanged += OnViewportHeightLevelChanged;

        BlockCoordsX = viewportViewModel.CameraPos.X.Floor();
        BlockCoordsY = viewportViewModel.HeightLevel;
        BlockCoordsZ = viewportViewModel.CameraPos.Z.Floor();
    }

    public IGotoViewModelClosedRecipient? ClosedRecipient { get; set; }
    public IClosable? Closable { get; set; }

    [RelayCommand]
    private void OnClosing()
    {
        _globalState.SavegameLoadInfoChanged -= OnSavegameLoadInfoChanged;
        _viewportViewModel.CameraPosChanged -= OnViewportCameraPosChanged;
        _viewportViewModel.HeightLevelChanged -= OnViewportHeightLevelChanged;
        ClosedRecipient?.Notify();
    }

    [RelayCommand]
    private void OnActivated() => _isActive = true;

    [RelayCommand]
    private void OnDeactivated() => _isActive = false;

    private void OnSavegameLoadInfoChanged(Models.SavegameLoadInfo? loadInfo)
    {
        if (loadInfo is null)
            Closable?.Close();
    }

    private void OnViewportCameraPosChanged()
    {
        var cameraPos = _viewportViewModel.CameraPos;

        BlockCoordsX = cameraPos.X.Floor();
        BlockCoordsZ = cameraPos.Z.Floor();
    }

    private void OnViewportHeightLevelChanged()
    {
        var heightLevel = _viewportViewModel.HeightLevel;
        BlockCoordsY = heightLevel;
    }

    partial void OnBlockCoordsXChanged(int value)
    {
        int chunkCoords = MathUtils.DivFloor(value, IChunk.BlockCount);
        ChunkCoordsX = chunkCoords;

        int regionCoords = MathUtils.DivFloor(chunkCoords, IRegion.ChunkCount);
        RegionCoordsX = regionCoords;

        var cameraPos = _viewportViewModel.CameraPos;
        cameraPos.X = value;

        if (_isActive)
            _viewportViewModel.CameraPos = cameraPos;
    }

    partial void OnBlockCoordsYChanged(int value)
    {
        int chunkCoords = MathUtils.DivFloor(value, IChunk.BlockCount);
        ChunkCoordsY = chunkCoords;

        if (_isActive)
            _viewportViewModel.HeightLevel = value;
    }

    partial void OnBlockCoordsZChanged(int value)
    {
        int chunkCoords = MathUtils.DivFloor(value, IChunk.BlockCount);
        ChunkCoordsZ = chunkCoords;

        int regionCoords = MathUtils.DivFloor(chunkCoords, IRegion.ChunkCount);
        RegionCoordsZ = regionCoords;

        var cameraPos = _viewportViewModel.CameraPos;
        cameraPos.Z = value;

        if (_isActive)
            _viewportViewModel.CameraPos = cameraPos;
    }


    partial void OnChunkCoordsXChanged(int value)
    {
        int blockCoordsMod = MathUtils.Mod(BlockCoordsX, IChunk.BlockCount);
        int blockCoords = value * IChunk.BlockCount + blockCoordsMod;
        BlockCoordsX = blockCoords;

        int regionCoords = MathUtils.DivFloor(value, IRegion.ChunkCount);
        RegionCoordsX = regionCoords;
    }

    partial void OnChunkCoordsYChanged(int value)
    {
        int blockCoordsMod = MathUtils.Mod(BlockCoordsY, IChunk.BlockCount);
        int blockCoords = value * IChunk.BlockCount + blockCoordsMod;
        BlockCoordsY = blockCoords;
    }

    partial void OnChunkCoordsZChanged(int value)
    {
        int blockCoordsMod = MathUtils.Mod(BlockCoordsZ, IChunk.BlockCount);
        int blockCoords = value * IChunk.BlockCount + blockCoordsMod;
        BlockCoordsZ = blockCoords;

        int regionCoords = MathUtils.DivFloor(value, IRegion.ChunkCount);
        RegionCoordsZ = regionCoords;
    }

    partial void OnRegionCoordsXChanged(int value)
    {
        int blockCoordsMod = MathUtils.Mod(BlockCoordsX, IRegion.BlockCount);
        int blockCoords = value * IRegion.BlockCount + blockCoordsMod;
        BlockCoordsX = blockCoords;

        int chunkCoordsMod = MathUtils.Mod(ChunkCoordsX, IRegion.ChunkCount);
        int chunkCoords = value * IRegion.ChunkCount + chunkCoordsMod;
        ChunkCoordsX = chunkCoords;
    }

    partial void OnRegionCoordsZChanged(int value)
    {
        int blockCoordsMod = MathUtils.Mod(BlockCoordsZ, IRegion.BlockCount);
        int blockCoords = value * IRegion.BlockCount + blockCoordsMod;
        BlockCoordsZ = blockCoords;

        int chunkCoordsMod = MathUtils.Mod(ChunkCoordsZ, IRegion.ChunkCount);
        int chunkCoords = value * IRegion.ChunkCount + chunkCoordsMod;
        ChunkCoordsZ = chunkCoords;
    }
}
