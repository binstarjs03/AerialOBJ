using binstarjs03.AerialOBJ.Core.CoordinateSystem;
using binstarjs03.AerialOBJ.Core.MinecraftWorld;

namespace binstarjs03.AerialOBJ.WpfApp;

public class ChunkWrapper
{
    private readonly Chunk _chunk;
    private readonly string[,] _highestBlocks = Chunk.GenerateHighestBlocksBuffer();

    public Coords2 ChunkCoordsAbs => _chunk.ChunkCoordsAbs;
    public Coords2 ChunkCoordsRel => _chunk.ChunkCoordsRel;
    public string[,] HighestBlocks => _highestBlocks;

    public ChunkWrapper(Chunk chunk)
    {
        _chunk = chunk;
    }
}
