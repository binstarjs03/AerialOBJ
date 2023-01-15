using binstarjs03.AerialOBJ.Core.Definitions;
using binstarjs03.AerialOBJ.Core.MinecraftWorld;
using binstarjs03.AerialOBJ.Core.Primitives;
using binstarjs03.AerialOBJ.WpfApp.Components;

namespace binstarjs03.AerialOBJ.WpfApp.Services.ChunkRendering;

public abstract class ChunkShaderBase : IChunkShader
{
    public abstract void RenderChunk(ViewportDefinition vd, IRegionImage regionImage, Block[,] highestBlocks, Point2Z<int> chunkCoordsRel);

    protected Color GetBlockColor(ViewportDefinition vd, in Block block)
    {
        // try get color from block definition else return missing block color
        string blockName = block.Name;
        if (vd.BlockDefinitions.TryGetValue(blockName, out ViewportBlockDefinition? bd))
            return bd.Color;
        else
            return vd.MissingBlockDefinition.Color;
    }
}
