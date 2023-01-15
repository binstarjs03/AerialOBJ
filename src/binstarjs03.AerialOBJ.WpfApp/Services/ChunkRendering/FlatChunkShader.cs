using binstarjs03.AerialOBJ.Core.Definitions;
using binstarjs03.AerialOBJ.Core.MinecraftWorld;
using binstarjs03.AerialOBJ.Core.Primitives;
using binstarjs03.AerialOBJ.WpfApp.Components;

namespace binstarjs03.AerialOBJ.WpfApp.Services.ChunkRendering;

/// <summary>
/// Chunk Shader that render blocks in a flat solid color
/// </summary>
public class FlatChunkShader : ChunkShaderBase
{
    public override void RenderChunk(ViewportDefinition vd, IRegionImage regionImage, Block[,] highestBlocks, Point2Z<int> chunkCoordsRel)
    {
        for (int x = 0; x < IChunk.BlockCount; x++)
            for (int z = 0; z < IChunk.BlockCount; z++)
            {
                Point2Z<int> blockCoordsRel = new(x, z);
                Point2<int> pixelCoords = ChunkRenderMath.GetRegionImagePixelCoords(chunkCoordsRel, blockCoordsRel);
                Color color = GetBlockColor(vd, in highestBlocks[x, z]);
                regionImage[pixelCoords.X, pixelCoords.Y] = color;
            }
    }
}
