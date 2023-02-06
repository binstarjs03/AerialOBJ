using binstarjs03.AerialOBJ.Core.Definitions;
using binstarjs03.AerialOBJ.Core.MinecraftWorld;
using binstarjs03.AerialOBJ.Core.Pooling;
using binstarjs03.AerialOBJ.Core.Primitives;

namespace binstarjs03.AerialOBJ.Imaging.ChunkRendering;
public class StandardChunkShaderV2 : ChunkShaderBase
{
    public override string ShaderName => "Standard V2";

    public override void RenderChunk(ChunkRenderOptions options)
    {
        BlockSlim[,] highestBlocks = GetChunkHighestBlock(options);
        ViewportDefinition vd = options.ViewportDefinition;

        for (int z = 0; z < IChunk.BlockCount; z++)
            for (int x = 0; x < IChunk.BlockCount; x++)
            {
                PointZ<int> blockCoordsRel = new(x, z);
                ref BlockSlim highestBlock = ref highestBlocks[x, z];

                if (highestBlock.Name == "minecraft:bubble_column")
                    highestBlock.Name = "minecraft:water";

                // if northern/western block is not possible (underflow index), then use self Y
                int northY = z > 0 ? highestBlocks[x, z - 1].Height : highestBlock.Height;
                int westY = x > 0 ? highestBlocks[x - 1, z].Height : highestBlock.Height;
                int northWestY = x > 0 && z > 0 ? highestBlocks[x - 1, z - 1].Height : highestBlock.Height;

                if (!vd.BlockDefinitions.TryGetValue(highestBlock.Name, out ViewportBlockDefinition? highestVbd))
                {
                    RenderMissingBlockDefinition(options, blockCoordsRel);
                    continue;
                }

                Color color = highestVbd.Color;

                // we can render directly if highest block is opaque
                if (highestVbd.Color.IsOpaque)
                {
                    color = ShadeColor(highestVbd.Color, in highestBlock, westY, northY, northWestY);
                    SetBlockPixelColorToImage(options, color, blockCoordsRel);
                    continue;
                }

                // else, keep looking down for opaque block
                BlockSlim lastBlock = highestBlock;
                while (true)
                {
                    if (lastBlock.Height == options.Chunk.LowestBlockHeight)
                        break;
                    BlockSlim block = options.Chunk.GetHighestBlockSlimSingleNoCheck(vd, blockCoordsRel, lastBlock.Height - 1, highestVbd.Name);

                    // we stop here if block definition is missing and we won't blend it 
                    if (!vd.BlockDefinitions.TryGetValue(block.Name, out ViewportBlockDefinition? blockVbd))
                        break;

                    int distance = lastBlock.Height - block.Height;
                    Color blockVbdColor = blockVbd.Color;
                    for (int i = 0; i < distance; i++)
                    {
                        blockVbdColor = ColorUtils.SimpleBlend(blockVbdColor, color, (byte)(color.Alpha / (i * 0.5 + 1f)));
                    }
                    color = blockVbdColor;

                    // we stop here if block color is opaque
                    if (blockVbd.Color.IsOpaque)
                        break;
                    lastBlock = block;
                }
                Color finalColor = ShadeColor(color, highestBlock, westY, northY, northWestY);
                SetBlockPixelColorToImage(options, finalColor, blockCoordsRel);
            }
        ReturnChunkHighestBlock(options, highestBlocks);
    }

    private static void RenderMissingBlockDefinition(ChunkRenderOptions options, PointZ<int> blockCoordsRel)
    {
        SetBlockPixelColorToImage(options, options.ViewportDefinition.MissingBlockDefinition.Color, blockCoordsRel);
    }

    private static Color ShadeColor(Color blockColor, in BlockSlim block, int westY, int northY, int northWestY)
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

        return new Color
        {
            Alpha = blockColor.Alpha,
            Red = (byte)Math.Clamp(blockColor.Red + difference, 0, 255),
            Green = (byte)Math.Clamp(blockColor.Green + difference, 0, 255),
            Blue = (byte)Math.Clamp(blockColor.Blue + difference, 0, 255),
        };
    }
}
