using binstarjs03.AerialOBJ.Core;
using binstarjs03.AerialOBJ.Core.MinecraftWorld;
using binstarjs03.AerialOBJ.Core.Primitives;

using CommunityToolkit.Mvvm.ComponentModel;

namespace binstarjs03.AerialOBJ.WpfApp.ViewModels;

[ObservableObject]
public partial class GotoViewModel
{
    [ObservableProperty] private int _blockCoordsX = 0;
    [ObservableProperty] private int _blockCoordsY = 0;
    [ObservableProperty] private int _blockCoordsZ = 0;

    [ObservableProperty] private int _chunkCoordsX = 0;
    [ObservableProperty] private int _chunkCoordsY = 0;
    [ObservableProperty] private int _chunkCoordsZ = 0;

    [ObservableProperty] private int _regionCoordsX = 0;
    [ObservableProperty] private int _regionCoordsZ = 0;

    partial void OnBlockCoordsXChanged(int value)
    {
        int chunkCoords = MathUtils.DivFloor(value, IChunk.BlockCount);
        int regionCoords = MathUtils.DivFloor(chunkCoords, IRegion.ChunkCount);

        _chunkCoordsX = chunkCoords;
        _regionCoordsX = regionCoords;

        OnPropertyChanged(nameof(ChunkCoordsX));
        OnPropertyChanged(nameof(RegionCoordsX));
    }

    partial void OnBlockCoordsYChanged(int value)
    {
        int chunkCoords = MathUtils.DivFloor(value, IChunk.BlockCount);

        _chunkCoordsY = chunkCoords;

        OnPropertyChanged(nameof(ChunkCoordsY));
    }


    partial void OnBlockCoordsZChanged(int value)
    {
        int chunkCoords = MathUtils.DivFloor(value, IChunk.BlockCount);
        int regionCoords = MathUtils.DivFloor(chunkCoords, IRegion.ChunkCount);

        _chunkCoordsZ = chunkCoords;
        _regionCoordsZ = regionCoords;

        OnPropertyChanged(nameof(ChunkCoordsZ));
        OnPropertyChanged(nameof(RegionCoordsZ));
    }


    partial void OnChunkCoordsXChanged(int value)
    {
        int blockCoordsMod = MathUtils.Mod(_blockCoordsX, IChunk.BlockCount);
        int blockCoords = value * IChunk.BlockCount + blockCoordsMod;
        int regionCoords = MathUtils.DivFloor(value, IRegion.ChunkCount);

        _blockCoordsX = blockCoords;
        _regionCoordsX = regionCoords;

        OnPropertyChanged(nameof(BlockCoordsX));
        OnPropertyChanged(nameof(RegionCoordsX));
    }

    partial void OnChunkCoordsYChanged(int value)
    {
        int blockCoordsMod = MathUtils.Mod(_blockCoordsY, IChunk.BlockCount);
        int blockCoords = value * IChunk.BlockCount + blockCoordsMod;

        _blockCoordsY = blockCoords;

        OnPropertyChanged(nameof(BlockCoordsY));
    }

    partial void OnChunkCoordsZChanged(int value)
    {
        int blockCoordsMod = MathUtils.Mod(_blockCoordsZ, IChunk.BlockCount);
        int blockCoords = value * IChunk.BlockCount + blockCoordsMod;
        int regionCoords = MathUtils.DivFloor(value, IRegion.ChunkCount);

        _blockCoordsZ = blockCoords;
        _regionCoordsZ = regionCoords;

        OnPropertyChanged(nameof(BlockCoordsZ));
        OnPropertyChanged(nameof(RegionCoordsZ));
    }

    partial void OnRegionCoordsXChanged(int value)
    {
        int blockCoordsMod = MathUtils.Mod(_blockCoordsX, IRegion.BlockCount);
        int blockCoords = value * IRegion.BlockCount + blockCoordsMod;
        int chunkCoordsMod = MathUtils.Mod(_chunkCoordsX, IRegion.ChunkCount);
        int chunkCoords = value * IRegion.ChunkCount + chunkCoordsMod;

        _blockCoordsX = blockCoords;
        _chunkCoordsX = chunkCoords;

        OnPropertyChanged(nameof(BlockCoordsX));
        OnPropertyChanged(nameof(ChunkCoordsX));
    }

    partial void OnRegionCoordsZChanged(int value)
    {
        int blockCoordsMod = MathUtils.Mod(_blockCoordsZ, IRegion.BlockCount);
        int blockCoords = value * IRegion.BlockCount + blockCoordsMod;
        int chunkCoordsMod = MathUtils.Mod(_chunkCoordsZ, IRegion.ChunkCount);
        int chunkCoords = value * IRegion.ChunkCount + chunkCoordsMod;

        _blockCoordsZ = blockCoords;
        _chunkCoordsZ = chunkCoords;

        OnPropertyChanged(nameof(BlockCoordsZ));
        OnPropertyChanged(nameof(ChunkCoordsZ));
    }
}
