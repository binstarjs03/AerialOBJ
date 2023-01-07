using binstarjs03.AerialOBJ.Core.Primitives;

namespace binstarjs03.AerialOBJ.Core.MinecraftWorld;
public interface IRegion
{
    Point2ZRange<int> ChunkRangeAbs { get; }
    Point2Z<int> Coords { get; }

    IChunk GetChunk(Point2Z<int> chunkCoordsRel);
    bool HasChunkGenerated(Point2Z<int> chunkCoordsRel);
}