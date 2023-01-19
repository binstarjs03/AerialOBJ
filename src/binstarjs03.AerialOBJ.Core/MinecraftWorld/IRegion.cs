using binstarjs03.AerialOBJ.Core.Primitives;

namespace binstarjs03.AerialOBJ.Core.MinecraftWorld;

/// <summary>
/// Abstraction for <see cref="Region"/>. 
/// Defined for easier unit testing
/// </summary>
public interface IRegion
{
    const int ChunkCount = 32;
    const int TotalChunkCount = ChunkCount * ChunkCount;
    const int ChunkRange = ChunkCount - 1;

    const int BlockCount = ChunkCount * IChunk.BlockCount;

    const int SectorDataLength = 4096;
    const int ChunkSectorTableSize = SectorDataLength;
    const int ChunkSectorTableEntrySize = 4;

    PointZRange<int> ChunkRangeAbs { get; }
    PointZ<int> Coords { get; }

    IChunk GetChunk(PointZ<int> chunkCoordsRel);
    bool HasChunkGenerated(PointZ<int> chunkCoordsRel);
}