using binstarjs03.AerialOBJ.Core.Definitions;
using binstarjs03.AerialOBJ.Core.MinecraftWorld;
using binstarjs03.AerialOBJ.Core.Primitives;

namespace binstarjs03.AerialOBJ.Imaging.ChunkRendering;
public class StandardChunkShaderV2 : ChunkShaderBase
{
    public override string ShaderName => "Standard V2";

    public override void RenderChunk(ChunkRenderOptions setting)
    {
        BlockSlim[,] highestBlocks = GetChunkHighestBlock(setting);
        for (int z = 0; z < IChunk.BlockCount; z++)
        {
            for (int x = 0; x < IChunk.BlockCount; x++)
            {
                // if northern/western block is not possible (underflow index), then use self Y
                int northY = z > 0 ? highestBlocks[x, z - 1].Height : highestBlocks[x, z].Height;
                int westY = x > 0 ? highestBlocks[x - 1, z].Height : highestBlocks[x, z].Height;
                int northWestY = x > 0 && z > 0 ? highestBlocks[x - 1, z - 1].Height : highestBlocks[x, z].Height;

                ref BlockSlim block = ref highestBlocks[x, z];
                Color color = RenderBlock(setting.ViewportDefinition, in block, westY, northY, northWestY);

                PointZ<int> blockCoordsRel = new(x, z);
                SetBlockPixelColorToImage(setting.Image, color, setting.RenderPosition, setting.Chunk.CoordsRel, blockCoordsRel);
            }
        }
        ReturnChunkHighestBlock(setting, highestBlocks);
    }

    private static Color RenderBlock(ViewportDefinition vd, in BlockSlim block, int westY, int northY, int northWestY)
    {
        /* pretend the sun is coming from northwest side
         * for example, lets say we have block arranged like so,
         * and we want to decide the shade for block center (surrounded in brackets):
         *      77,  75 , 78
         *      76, [74], 73
         *      75,  75 , 76
         *
         * since block bracket Y = 74 is lower than both of its north and west block (75 and 76 respectively),
         * then block bracket is darker because the sun shine from northwest, so the shadow cast to southeast
         */

        int difference = 0;
        int selfY = block.Height;

        // since the sun is coming from northwest side, if y of this block is higher
        // than either west block (last y) or north block (last y row at this index)
        // set shade to brighter, else dimmer if lower, else keep it as is if same

        // TODO check if block is foliage instead of hardcoding grass!!!
        if (block.Name == "minecraft:grass")
            selfY -= 1;
        else if (block.Name == "minecraft:tall_grass")
            selfY -= 2;

        if (selfY > westY)
            difference += 10;
        else if (selfY < westY)
            difference -= 10;

        if (selfY > northY)
            difference += 10;
        else if (selfY < northY)
            difference -= 10;

        if (selfY > northWestY)
            difference += 20;
        else if (selfY < northWestY)
            difference -= 20;

        Color color = GetBlockColor(vd, in block);
        color.Red = (byte)Math.Clamp(color.Red + difference, 0, 255);
        color.Green = (byte)Math.Clamp(color.Green + difference, 0, 255);
        color.Blue = (byte)Math.Clamp(color.Blue + difference, 0, 255);

        return color;
    }
}
