using binstarjs03.AerialOBJ.Core.MinecraftWorld;
using binstarjs03.AerialOBJ.Core.Primitives;

namespace binstarjs03.AerialOBJ.WpfApp.Services.ChunkRendering;
public static class ChunkRenderMath
{
    public static PointY<int> GetRegionImagePixelCoords(PointZ<int> chunkCoordsRel, PointZ<int> blockCoordsRel)
    {
        return new PointY<int>(chunkCoordsRel.X * IChunk.BlockCount + blockCoordsRel.X,
                               chunkCoordsRel.Z * IChunk.BlockCount + blockCoordsRel.Z);
    }
}
