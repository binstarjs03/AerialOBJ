using binstarjs03.AerialOBJ.Core.Definitions;
using binstarjs03.AerialOBJ.Core.MinecraftWorld;
using binstarjs03.AerialOBJ.Core.Pooling;
using binstarjs03.AerialOBJ.Core.Primitives;

namespace binstarjs03.AerialOBJ.Imaging.ChunkRendering;
public class StandardChunkShader : ChunkShaderBase
{
    private readonly ObjectPool<List<BlockColorLayer>> _blockLayersPooler = new();

    public override string ShaderName => "Standard V2";

    public override void RenderChunk(ChunkRenderOptions options)
    {
        var highestBlocks = GetChunkHighestBlock(options);

        for (int z = 0; z < IChunk.BlockCount; z++)
            for (int x = 0; x < IChunk.BlockCount; x++)
            {
                PointZ<int> blockCoordsRel = new(x, z);
                RenderBlock(options, highestBlocks, blockCoordsRel);
            }

        ReturnChunkHighestBlock(options, highestBlocks);
    }

    private void RenderBlock(ChunkRenderOptions options, BlockSlim[,] highestBlocks, PointZ<int> blockCoordsRel)
    {
        Color finalPixelColor;
        var viewportDefinition = options.ViewportDefinition;
        ref var highestBlock = ref highestBlocks[blockCoordsRel.X, blockCoordsRel.Z];
        int lowestBlockHeight = options.Chunk.LowestBlockHeight;

        if (!viewportDefinition.BlockDefinitions.TryGetValue(highestBlock.Name, out var highestBlockDefinition))
        {
            RenderMissingBlockDefinition(options, blockCoordsRel);
            return;
        }

        (int northY, int westY, int northWestY) = GetNeighboringBlockHeight(highestBlocks, highestBlock.Height, blockCoordsRel);

        // Render block color directly if highest block color is opaque
        if (highestBlockDefinition.Color.IsOpaque || highestBlock.Height <= lowestBlockHeight)
        {
            finalPixelColor = ShadeBlockColor(highestBlockDefinition.Color, in highestBlock, westY, northY, northWestY);
            SetImagePixel(options, finalPixelColor, blockCoordsRel);
            return;
        }

        // Highest block color is not opaque, combine all semitransparent block colors below it.
        // The way we do that is by building layer of blocks, scanning the chunk to the bottom until opaque block is found

        var blockLayers = RentOrCreateBlockLayers();
        Color background;

        var lastBlock = highestBlock;
        var lastBlockDefinition = highestBlockDefinition;
        do
        {
            // rescan block until different block is found, so we exclude last block
            var block = options.Chunk.GetHighestBlockSlimSingleNoCheck(viewportDefinition, blockCoordsRel, lastBlock.Height - 1, lastBlock.Name);

            var hasBlockDefinition = viewportDefinition.BlockDefinitions.TryGetValue(block.Name, out var blockDefinition);
            if (!hasBlockDefinition)
                blockDefinition = viewportDefinition.MissingBlockDefinition;

            int distance = lastBlock.Height - block.Height;
            blockLayers.Add(new BlockColorLayer
            {
                Color = lastBlockDefinition.Color,
                LayerThickness = distance,
            });

            // if found opaque block color, we stop here
            if (blockDefinition!.Color.IsOpaque || block.Height <= lowestBlockHeight)
            {
                background = blockDefinition.Color;
                break;
            }

            lastBlock = block;
            lastBlockDefinition = blockDefinition;
        } while (true);

        // well we did scanning from top to bottom while in image layering
        // layer combinations are done from bottom up to the top,
        // our layer direction is wrong so we reverse it
        blockLayers.Reverse();

        Color combinedBlockColors = CombineLayersOfBlockColor(background, blockLayers);
        finalPixelColor = ShadeBlockColor(combinedBlockColors, highestBlock, westY, northY, northWestY);
        SetImagePixel(options, finalPixelColor, blockCoordsRel);

        blockLayers.Clear();
        ReturnBlockLayers(blockLayers);
    }

    private static Color CombineLayersOfBlockColor(Color background, IList<BlockColorLayer> blockLayers)
    {
        Color result = background;
        foreach (BlockColorLayer layer in blockLayers)
            for (int i = 0; i < layer.LayerThickness; i++)
            {
                // map alpha value from (0 - 255) to (0 - 1)
                float alphaRemap = layer.Color.Alpha / (float)byte.MaxValue;

                // we want to blend the colors more or less logarithmically.
                // we may also expose strength to the setting so transparency
                // blending can be adjusted to the user preference
                float strength = 0.5f;
                float ratio = alphaRemap / (i * strength + 1);

                result = ColorUtils.SimpleBlend(result, layer.Color, ratio);
            }
        return result;
    }

    private static (int northY, int westY, int northWestY) GetNeighboringBlockHeight(BlockSlim[,] highestBlocks, int selfHeight, PointZ<int> blockCoordsRel)
    {
        int x = blockCoordsRel.X;
        int z = blockCoordsRel.Z;

        // if neighboring block is not possible (underflow index), then use self Y
        int northY = z > 0 ? highestBlocks[x, z - 1].Height : selfHeight;
        int westY = x > 0 ? highestBlocks[x - 1, z].Height : selfHeight;
        int northWestY = x > 0 && z > 0 ? highestBlocks[x - 1, z - 1].Height : selfHeight;
        return (northY, westY, northWestY);
    }

    private static void RenderMissingBlockDefinition(ChunkRenderOptions options, PointZ<int> blockCoordsRel)
    {
        SetImagePixel(options, options.ViewportDefinition.MissingBlockDefinition.Color, blockCoordsRel);
    }

    private List<BlockColorLayer> RentOrCreateBlockLayers()
    {
        if (!_blockLayersPooler.Rent(out var blockLayers))
            blockLayers = new List<BlockColorLayer>();
        return blockLayers;
    }

    private void ReturnBlockLayers(List<BlockColorLayer> blockLayers)
    {
        _blockLayersPooler.Return(blockLayers);
    }

    private static Color ShadeBlockColor(Color blockColor, in BlockSlim block, int westY, int northY, int northWestY)
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

    /// <summary>
    /// Represent stacked same Minecraft block color on top of each other
    /// </summary>
    private struct BlockColorLayer
    {
        public required Color Color { get; set; }
        public required int LayerThickness { get; set; }
    }
}
