using binstarjs03.AerialOBJ.Core.Primitives;

namespace binstarjs03.AerialOBJ.Core.MinecraftWorldRefactor;
public class Chunk : IChunk
{
    public Point2Z<int> CoordsRel { get; }
    public Point2Z<int> CoordsAbs { get; }

    public int DataVersion => throw new System.NotImplementedException();

    public string ReleaseVersion => throw new System.NotImplementedException();

    public void GetHighestBlock(ChunkHighestBlockBuffer buffer)
    {
        throw new System.NotImplementedException();
    }
}
