using binstarjs03.AerialOBJ.Core.Definitions;
using binstarjs03.AerialOBJ.Core.MinecraftWorld;
using binstarjs03.AerialOBJ.Core.Primitives;
using binstarjs03.AerialOBJ.WpfApp.Components;

namespace binstarjs03.AerialOBJ.WpfApp.Services.ChunkRendering;

public abstract class ChunkShaderBase : IChunkShader
{
    protected readonly IDefinitionManagerService _definitionManager;

    protected ChunkShaderBase(IDefinitionManagerService definitionManager)
    {
        _definitionManager = definitionManager;
    }

    public abstract void RenderChunk(IRegionImage regionImage, Block[,] highestBlocks, Point2Z<int> chunkCoordsRel);

    protected Color GetBlockColor(in Block block)
    {
        // try get color from block definition else return missing block color
        string blockName = block.Name;
        if (_definitionManager.CurrentViewportDefinition.BlockDefinitions.TryGetValue(blockName, out ViewportBlockDefinition? bd))
            return bd.Color;
        else
            return _definitionManager.CurrentViewportDefinition.MissingBlockDefinition.Color;
    }
}
