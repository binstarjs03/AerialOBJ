using System;

using binstarjs03.AerialOBJ.Core.Definitions;
using binstarjs03.AerialOBJ.Core.MinecraftWorld;
using binstarjs03.AerialOBJ.Core.Primitives;
using binstarjs03.AerialOBJ.WpfApp.Components;

namespace binstarjs03.AerialOBJ.WpfApp.Services.ChunkRendering;
public class StandardChunkShader : ChunkShaderBase
{
    public override void RenderChunk(ViewportDefinition vd, IRegionImage regionImage, BlockSlim[,] highestBlocks, Point2Z<int> chunkCoordsRel)
    {
        // pretend the sun is coming from northwest side/
        // for example, lets say we have block arranged like so,
        // and we want to decide the shade for block center (surrounded in brackets):

        /*      77,  75 , 78
         *      76, [74], 73
         *      75,  75 , 76
         */

        // since block bracket Y = 74 is lower than both of its north and west block (75 and 76 respectively),
        // then block bracket is darker because the sun shine from northwest, so the shadow cast to southeast

        // initialize row of blocks to the first row (X = X, Z = 0)
        Span<int> lastYRow = stackalloc int[IChunk.BlockCount];
        for (int x = 0; x < IChunk.BlockCount; x++)
            lastYRow[x] = highestBlocks[x, 0].Height;

        for (int z = 0; z < IChunk.BlockCount; z++)
        {
            // initialize lastY to the first block
            int lastY = highestBlocks[0, z].Height;
            for (int x = 0; x < IChunk.BlockCount; x++)
            {
                ref BlockSlim block = ref highestBlocks[x, z];
                Color color = GetBlockColor(vd, in block);

                // since the sun is coming from northwest side, if y of this block is higher
                // than either west block (last y) or north block (last y row at this index)
                // set shade to brighter, else dimmer if lower, else keep it as is if same
                int difference = 0;
                int y = block.Height;
                int lastYrow = lastYRow[x];

                // TODO check if block is foliage instead of hardcoding grass!!!
                if (block.Name == "minecraft:grass" || block.Name == "minecraft:tall_grass")
                    y -= 1;

                if (y > lastY || y > lastYRow[x])
                {
                    if (y > lastY && y > lastYRow[x])
                        difference = 30;
                    else
                        difference = 15;
                }
                else if (y < lastY || y < lastYRow[x])
                {
                    if (y < lastY && y < lastYRow[x])
                        difference = -30;
                    else
                        difference = -15;
                }

                color.Red = (byte)Math.Clamp(color.Red + difference, 0, 255);
                color.Green = (byte)Math.Clamp(color.Green + difference, 0, 255);
                color.Blue = (byte)Math.Clamp(color.Blue + difference, 0, 255);

                Point2Z<int> blockCoordsRel = new(x, z);
                Point2<int> pixelCoords = ChunkRenderMath.GetRegionImagePixelCoords(chunkCoordsRel, blockCoordsRel);
                regionImage[pixelCoords.X, pixelCoords.Y] = color;

                lastY = block.Height;
                lastYRow[x] = block.Height;
            }
        }
    }
}
