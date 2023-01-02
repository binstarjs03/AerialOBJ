using binstarjs03.AerialOBJ.Core.MinecraftWorld;
using binstarjs03.AerialOBJ.Core.Primitives;

namespace binstarjs03.AerialOBJ.WpfApp.Services.ChunkRendering;
public static class ChunkRenderMath
{
    public static Point2<int> GetRegionImagePixelCoords(Point2Z<int> chunkCoordsRel, Point2Z<int> blockCoordsRel)
    {
        return new Point2<int>(chunkCoordsRel.X * Section.BlockCount + blockCoordsRel.X,
                               chunkCoordsRel.Z * Section.BlockCount + blockCoordsRel.Z);
    }
}
