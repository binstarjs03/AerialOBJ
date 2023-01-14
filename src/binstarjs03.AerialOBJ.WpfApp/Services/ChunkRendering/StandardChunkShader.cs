using System;

using binstarjs03.AerialOBJ.Core.MinecraftWorld;
using binstarjs03.AerialOBJ.Core.Primitives;
using binstarjs03.AerialOBJ.WpfApp.Components;

namespace binstarjs03.AerialOBJ.WpfApp.Services.ChunkRendering;
public class StandardChunkShader : ChunkShaderBase
{
    public StandardChunkShader(IDefinitionManagerService definitionManager) : base(definitionManager) { }

    public override void RenderChunk(IRegionImage regionImage, Block[,] highestBlocks, Point2Z<int> chunkCoordsRel)
    {
        // pretend the sun is coming from northwest side

        // initialize row of blocks to the first row (X = X, Z = 1)
        Span<int> lastYRow = stackalloc int[IChunk.BlockCount];
        for (int x = 0; x < IChunk.BlockCount; x++)
            lastYRow[x] = highestBlocks[x, 0].Coords.Y;

        for (int z = 0; z < IChunk.BlockCount; z++)
        {
            // initialize lastY to the first block
            int lastY = highestBlocks[0, z].Coords.Y;
            for (int x = 0; x < IChunk.BlockCount; x++)
            {
                // no copying struct!
                ref Block block = ref highestBlocks[x, z];
                Color color = GetBlockColor(in block);

                // since the sun is coming from northwest side, if y is higher than last
                // set shade to brighter, else dimmer if lower, else keep it as is if same
                int difference = 0;
                int y = block.Coords.Y;
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

                lastY = block.Coords.Y;
                lastYRow[x] = block.Coords.Y;
            }
        }
    }
}
