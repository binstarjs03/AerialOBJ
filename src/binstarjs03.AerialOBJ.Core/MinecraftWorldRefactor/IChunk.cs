using binstarjs03.AerialOBJ.Core.Primitives;

namespace binstarjs03.AerialOBJ.Core.MinecraftWorldRefactor;
public interface IChunk
{
    const int BlockCount = 16;
    const int BlockRange = BlockCount - 1;

    int DataVersion { get; }
    string ReleaseVersion { get; } // example: "1.18.2"
    Point2Z<int> CoordsAbs { get; }
    Point2Z<int> CoordsRel { get; }

    void GetHighestBlock(Block[,] buffer);
}