using binstarjs03.AerialOBJ.Core;
using binstarjs03.AerialOBJ.Core.MinecraftWorld;
using binstarjs03.AerialOBJ.Core.Primitives;

using CommunityToolkit.Mvvm.ComponentModel;

namespace binstarjs03.AerialOBJ.WpfApp.ViewModels;

[ObservableObject]
public partial class GotoViewModel
{
    private Point3<int> _blockCoords = Point3<int>.Zero;
    private Point3<int> _chunkCoords = Point3<int>.Zero;
    private PointZ<int> _regionCoords = PointZ<int>.Zero;

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


    public Point3<int> BlockCoords
    {
        get => _blockCoords;
        set
        {
            if (value == _blockCoords)
                return;
            _blockCoords = value;
            var chunkCoords = MinecraftWorldMathUtils.GetChunkCoordsAbsFromBlockCoordsAbs(value);
            _chunkCoords = chunkCoords;
            _regionCoords = MinecraftWorldMathUtils.GetRegionCoordsFromChunkCoordsAbs(chunkCoords);
            OnPropertyChanged();
            OnPropertyChanged(nameof(ChunkCoords));
            OnPropertyChanged(nameof(RegionCoords));
        }
    }
    public Point3<int> ChunkCoords
    {
        get => _chunkCoords;
        set
        {
            if (value == _chunkCoords)
                return;
            _chunkCoords = value;
        }
    }
    public PointZ<int> RegionCoords
    {
        get => _regionCoords;
        set
        {
            if (value == _regionCoords)
                return;
            _regionCoords = value;
        }
    }
}
