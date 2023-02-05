using binstarjs03.AerialOBJ.Core.MinecraftWorld;
using binstarjs03.AerialOBJ.Core.Primitives;

namespace binstarjs03.AerialOBJ.Imaging.ChunkRendering;
public class FlatChunkShader : ChunkShaderBase
{
    public override string ShaderName => "Flat";

    public override void RenderChunk(ChunkRenderOptions options)
    {
        BlockSlim[,] highestBlocks = GetChunkHighestBlock(options);
        for (int x = 0; x < IChunk.BlockCount; x++)
            for (int z = 0; z < IChunk.BlockCount; z++)
            {
                PointZ<int> blockCoordsRel = new(x, z);
                Color color = GetBlockColor(options.ViewportDefinition, in highestBlocks[x, z]);
                color.MakeOpaque();
                SetBlockPixelColorToImage(options, color, blockCoordsRel);
            }
        ReturnChunkHighestBlock(options, highestBlocks);
    }
}
