using binstarjs03.AerialOBJ.Core;
using binstarjs03.AerialOBJ.Core.MinecraftWorld;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace binstarjs03.AerialOBJ.MVVM.ViewModels;
public partial class GotoViewModel : ObservableObject
{
    [ObservableProperty] private int _blockCoordsX = 0;
    [ObservableProperty] private int _blockCoordsY = 0;
    [ObservableProperty] private int _blockCoordsZ = 0;

    [ObservableProperty] private int _chunkCoordsX = 0;
    [ObservableProperty] private int _chunkCoordsY = 0;
    [ObservableProperty] private int _chunkCoordsZ = 0;

    [ObservableProperty] private int _regionCoordsX = 0;
    [ObservableProperty] private int _regionCoordsZ = 0;

    public IGotoViewModelClosedRecipient? ClosedRecipient { get; set; }

    partial void OnBlockCoordsXChanged(int value)
    {
        int chunkCoords = MathUtils.DivFloor(value, IChunk.BlockCount);
        ChunkCoordsX = chunkCoords;

        int regionCoords = MathUtils.DivFloor(chunkCoords, IRegion.ChunkCount);
        RegionCoordsX = regionCoords;
    }

    partial void OnBlockCoordsYChanged(int value)
    {
        int chunkCoords = MathUtils.DivFloor(value, IChunk.BlockCount);
        ChunkCoordsY = chunkCoords;
    }

    partial void OnBlockCoordsZChanged(int value)
    {
        int chunkCoords = MathUtils.DivFloor(value, IChunk.BlockCount);
        ChunkCoordsZ = chunkCoords;

        int regionCoords = MathUtils.DivFloor(chunkCoords, IRegion.ChunkCount);
        RegionCoordsZ = regionCoords;
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

    [RelayCommand]
    void OnClosing() => ClosedRecipient?.Notify();
}
