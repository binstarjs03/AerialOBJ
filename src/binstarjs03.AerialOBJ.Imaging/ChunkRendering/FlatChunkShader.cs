using binstarjs03.AerialOBJ.Core.MinecraftWorld;
using binstarjs03.AerialOBJ.Core.Primitives;

namespace binstarjs03.AerialOBJ.Imaging.ChunkRendering;
public class FlatChunkShader : ChunkShaderBase
{
    public override void RenderChunk(ChunkRenderSetting setting)
    {
        BlockSlim[,] highestBlocks = setting.HighestBlocks ?? GetChunkHighestBlock(setting);
        for (int x = 0; x < IChunk.BlockCount; x++)
            for (int z = 0; z < IChunk.BlockCount; z++)
            {
                PointZ<int> blockCoordsRel = new(x, z);
                Color color = GetBlockColor(setting.ViewportDefinition, in highestBlocks[x, z]);
                SetBlockPixelColorToImage(setting.Image, color, setting.RenderPosition, setting.Chunk.CoordsRel, blockCoordsRel);
            }
        ReturnChunkHighestBlock(setting, highestBlocks);
    }
}
