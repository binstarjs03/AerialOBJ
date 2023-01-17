using binstarjs03.AerialOBJ.Core.Definitions;
using binstarjs03.AerialOBJ.Core.MinecraftWorld;
using binstarjs03.AerialOBJ.Core.Primitives;
using binstarjs03.AerialOBJ.WpfApp.Components;

namespace binstarjs03.AerialOBJ.WpfApp.Services.ChunkRendering;

public abstract class ChunkShaderBase : IChunkShader
{
    public abstract void RenderChunk(ViewportDefinition vd, IRegionImage regionImage, BlockSlim[,] highestBlocks, Point2Z<int> chunkCoordsRel);

    protected static Color GetBlockColor(ViewportDefinition vd, in BlockSlim block)
    {
        // try get color from block definition else return missing block color
        string blockName = block.Name;
        if (vd.BlockDefinitions.TryGetValue(blockName, out ViewportBlockDefinition? bd))
            return bd.Color;
        else
            return vd.MissingBlockDefinition.Color;
    }
}
