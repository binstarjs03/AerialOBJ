using binstarjs03.AerialOBJ.Core.MinecraftWorld;
using binstarjs03.AerialOBJ.Core.Pooling;
using binstarjs03.AerialOBJ.Core.Primitives;

namespace binstarjs03.AerialOBJ.Imaging.ChunkRendering;
public class StandardChunkShader : ChunkShaderBase
{
    private readonly ObjectPool<List<BlockColorLayer>> _blockLayersPooler = new();

    public override string ShaderName => "Standard";

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
        Color mergedTransparencies = MergeTransparentBlockColors(options, highestBlocks, blockCoordsRel);
        Color shaded = ShadeBlockColor(mergedTransparencies, highestBlocks, blockCoordsRel);
        SetImagePixel(options, shaded, blockCoordsRel);
    }

    private Color MergeTransparentBlockColors(ChunkRenderOptions options, BlockSlim[,] highestBlocks, PointZ<int> blockCoordsRel)
    {
        var viewportDefinition = options.ViewportDefinition;
        int lowestBlockHeight = options.Chunk.LowestBlockHeight;

        ref var highestBlock = ref highestBlocks[blockCoordsRel.X, blockCoordsRel.Z];
        var highestBlockDefinition = highestBlock.GetBlockDefinitionOrDefault(viewportDefinition);

        // return immediately if highest block color is opaque
        if (highestBlockDefinition.Color.IsOpaque || highestBlock.Height <= lowestBlockHeight)
            return highestBlockDefinition.Color;

        // Combine all semitransparent block colors. The way we do this is by building
        // layer of block, scanning the chunk to the bottom until opaque block color is found
        var blockLayers = RentOrCreateBlockLayers();

        var lastBlock = highestBlock;
        var lastBlockDefinition = highestBlockDefinition;
        do
        {
            // rescan block until different block is found, so we exclude last block
            var block = options.Chunk.GetHighestBlockSlimSingleNoCheck(viewportDefinition,
                                                                       blockCoordsRel,
                                                                       lastBlock.Height - 1,
                                                                       lastBlock.Name);
            var blockDefinition = block.GetBlockDefinitionOrDefault(viewportDefinition);
            int distance = lastBlock.Height - block.Height;

            blockLayers.Add(new BlockColorLayer
            {
                Color = lastBlockDefinition.Color,
                LayerThickness = distance,
            });

            lastBlock = block;
            lastBlockDefinition = blockDefinition;

        } while (!lastBlockDefinition.Color.IsOpaque && lastBlock.Height > lowestBlockHeight);

        Color background = lastBlockDefinition.Color;
        Color result = CombineLayersOfBlockColor(background, blockLayers);

        blockLayers.Clear();
        ReturnBlockLayers(blockLayers);
        return result;
    }

    private static Color CombineLayersOfBlockColor(Color background, IList<BlockColorLayer> blockLayers)
    {
        Color result = background;

        // In most photo editing software, layer merging is done from bottom to the top
        // so we iterate block layers reversely
        for (int layerIndex = blockLayers.Count - 1; layerIndex >= 0; layerIndex--)
        {
            BlockColorLayer layer = blockLayers[layerIndex];

            // repeatedly combine same layer by thickness time
            for (int repeatIndex = 0; repeatIndex < layer.LayerThickness; repeatIndex++)
            {
                // map alpha value from (0 - 255) to (0 - 1)
                float alphaRemap = layer.Color.Alpha / (float)byte.MaxValue;

                // we want to blend the colors more or less logarithmically.
                // we may also expose strength to the setting so transparency
                // blending can be adjusted to the user preference
                float strength = 0.5f;
                float ratio = alphaRemap / (repeatIndex * strength + 1);

                result = ColorUtils.SimpleBlend(result, layer.Color, ratio);
            }
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

    private static Color ShadeBlockColor(Color blockColor, BlockSlim[,] highestBlocks, PointZ<int> blockCoordsRel)
    {
        var block = highestBlocks[blockCoordsRel.X, blockCoordsRel.Z];
        (int northY, int westY, int northWestY) = GetNeighboringBlockHeight(highestBlocks, block.Height, blockCoordsRel);

        // we shade the color by comparing if this block Y is higher or lower than its neighboring block height
        // since the sun is coming from northwest, we sample north, northwest and west block

        int differenceDelta = 15;
        int heightDifferenceLimit = 1;
        int selfY = block.Height;

        int westDifference = Math.Clamp(selfY - westY, -heightDifferenceLimit, heightDifferenceLimit);
        int northDifference = Math.Clamp(selfY - northY, -heightDifferenceLimit, heightDifferenceLimit);
        int northWestDifference = Math.Clamp(selfY - northWestY, -heightDifferenceLimit, heightDifferenceLimit);

        int difference = westDifference * differenceDelta 
                       + northDifference * differenceDelta
                       + northWestDifference * differenceDelta;

        return new Color
        {
            Alpha = blockColor.Alpha,
            Red = (byte)Math.Clamp(blockColor.Red + difference, byte.MinValue, byte.MaxValue),
            Green = (byte)Math.Clamp(blockColor.Green + difference, byte.MinValue, byte.MaxValue),
            Blue = (byte)Math.Clamp(blockColor.Blue + difference, byte.MinValue, byte.MaxValue),
        };
    }

    /// <summary>
    /// Represent Minecraft block color, repeatedly stacked on top of itself
    /// </summary>
    private struct BlockColorLayer
    {
        public required Color Color { get; set; }
        public required int LayerThickness { get; set; }
    }
}
