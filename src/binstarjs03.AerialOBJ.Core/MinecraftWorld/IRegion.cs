using binstarjs03.AerialOBJ.Core.Primitives;

namespace binstarjs03.AerialOBJ.Core.MinecraftWorld;
public interface IRegion
{
    const int ChunkCount = 32;
    const int TotalChunkCount = ChunkCount * ChunkCount;
    const int ChunkRange = ChunkCount - 1;

    const int BlockCount = ChunkCount * IChunk.BlockCount;

    const int SectorDataLength = 4096;
    const int ChunkSectorTableSize = SectorDataLength;
    const int ChunkSectorTableEntrySize = 4;

    Point2ZRange<int> ChunkRangeAbs { get; }
    Point2Z<int> Coords { get; }

    IChunk GetChunk(Point2Z<int> chunkCoordsRel);
    bool HasChunkGenerated(Point2Z<int> chunkCoordsRel);
}