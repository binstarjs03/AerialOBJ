using binstarjs03.AerialOBJ.Core.MinecraftWorld;
using binstarjs03.AerialOBJ.Core.Pooling;
using binstarjs03.AerialOBJ.Core.Primitives;

namespace binstarjs03.AerialOBJ.Imaging.ChunkRendering;
public class StandardChunkShaderV2 : ChunkShaderBase
{
    private readonly ObjectPool<List<(Color, int)>> _blockListPooler = new();

    public override string ShaderName => "Standard V2";

    public override void RenderChunk(ChunkRenderOptions options)
    {
        var colorList = GetColorList();
        var highestBlocks = GetChunkHighestBlock(options);

        for (int z = 0; z < IChunk.BlockCount; z++)
            for (int x = 0; x < IChunk.BlockCount; x++)
            {
                PointZ<int> blockCoordsRel = new PointZ<int>(x, z);
                RenderBlock(options, highestBlocks, colorList, blockCoordsRel);
            }

        ReturnChunkHighestBlock(options, highestBlocks);
        ReturnColorList(colorList);
    }

    private static void RenderBlock(ChunkRenderOptions options, BlockSlim[,] highestBlocks, List<(Color, int)> colorLayers, PointZ<int> blockCoordsRel)
    {
        Color finalPixelColor;
        var vd = options.ViewportDefinition;
        ref var highestBlock = ref highestBlocks[blockCoordsRel.X, blockCoordsRel.Z];

        if (!vd.BlockDefinitions.TryGetValue(highestBlock.Name, out var highestVbd))
        {
            RenderMissingBlockDefinition(options, blockCoordsRel);
            return;
        }

        (int northY, int westY, int northWestY) = GetNeighboringBlockHeight(highestBlocks, highestBlock.Height, blockCoordsRel);

        // we can render directly if highest block is opaque or highest block is lowest block
        if (highestVbd.Color.IsOpaque || highestBlock.Height <= options.Chunk.LowestBlockHeight)
        {
            finalPixelColor = ShadeColor(highestVbd.Color, in highestBlock, westY, northY, northWestY);
            SetBlockPixelColorToImage(options, finalPixelColor, blockCoordsRel);
            return;
        }

        var lastBlock = highestBlock;
        var lastVbd = highestVbd;
        do
        {
            // rescan block until different block is found, so we exclude last block
            var block = options.Chunk.GetHighestBlockSlimSingleNoCheck(vd, blockCoordsRel, lastBlock.Height - 1, lastBlock.Name);

            var hasBlockDefinition = vd.BlockDefinitions.TryGetValue(block.Name, out var blockVbd);
            if (!hasBlockDefinition)
                blockVbd = vd.MissingBlockDefinition;

            int distance = lastBlock.Height - block.Height;
            colorLayers.Add((lastVbd.Color, distance));

            if (blockVbd!.Color.IsOpaque || !hasBlockDefinition || block.Height == options.Chunk.LowestBlockHeight)
            {
                // add last block color to color layers
                colorLayers.Add((blockVbd.Color, 0));
                break;
            }

            lastBlock = block;
            lastVbd = blockVbd;

        } while (true);

        Color combinedColorLayers = CombineColorLayers(colorLayers);
        finalPixelColor = ShadeColor(combinedColorLayers, highestBlock, westY, northY, northWestY);
        SetBlockPixelColorToImage(options, finalPixelColor, blockCoordsRel);
        colorLayers.Clear();
    }

    private static Color CombineColorLayers(List<(Color color, int count)> colorLayers)
    {
        colorLayers.Reverse();

        // set initial color to the first layer (bottom block),
        // which we expect to be opaque
        Color result = colorLayers[0].color;

        // because we already put the first layer, we skip the first layer
        // in the loop, so we set initial index to one, not zero
        for (int i = 1; i < colorLayers.Count; i++)
        {
            Color nextLayer = colorLayers[i].color;
            int count = colorLayers[i].count;
            result = RepeatingCombineColorLayer(result, nextLayer, count);
        }

        return result;
    }

    /// <summary>
    /// Repeatedly combine color <paramref name="source"/>, <paramref name="count"/> 
    /// times to <paramref name="destination"/>
    /// </summary>
    private static Color RepeatingCombineColorLayer(Color destination, Color source, int count)
    {
        for (int i = 0; i < count; i++)
            destination = ColorUtils.SimpleBlend(destination, source, source.Alpha);
        return destination;
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
        SetBlockPixelColorToImage(options, options.ViewportDefinition.MissingBlockDefinition.Color, blockCoordsRel);
    }

    private List<(Color, int)> GetColorList()
    {
        if (!_blockListPooler.Rent(out var colorList))
            colorList = new List<(Color, int)>();
        return colorList;
    }

    private void ReturnColorList(List<(Color, int)> colorList)
    {
        _blockListPooler.Return(colorList);
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
