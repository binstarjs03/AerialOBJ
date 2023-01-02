using System.Threading;

using binstarjs03.AerialOBJ.Core.Definitions;
using binstarjs03.AerialOBJ.Core.MinecraftWorldRefactor;
using binstarjs03.AerialOBJ.Core.Primitives;
using binstarjs03.AerialOBJ.WpfApp.Models;

namespace binstarjs03.AerialOBJ.WpfApp.Services.ChunkRendering;
/// <summary>
/// Chunk Shader that render blocks in a flat solid color
/// </summary>
public class FlatChunkShader : IChunkShader
{
    private readonly DefinitionManagerService _definitionManager;

    public FlatChunkShader(DefinitionManagerService definitionManager)
    {
        _definitionManager = definitionManager;
    }

    public void RenderChunk(RegionModel regionModel, Block[,] highestBlocks, Point2Z<int> chunkCoordsRel, CancellationToken cancellationToken)
    {
        for (int x = 0; x < IChunk.BlockCount; x++)
            for (int z = 0; z < IChunk.BlockCount; z++)
            {
                Point2Z<int> blockCoordsRel = new(x, z);
                Point2<int> pixelCoords = ChunkRenderMath.GetRegionImagePixelCoords(chunkCoordsRel, blockCoordsRel);
                Color color = GetBlockColor(_definitionManager.DefaultViewportDefinition, highestBlocks, blockCoordsRel);
                regionModel.RegionImage[pixelCoords.X, pixelCoords.Y] = color;
            }
    }

    private static Color GetBlockColor(ViewportDefinition definition, Block[,] highestBlocks, Point2Z<int> blockCoordsRel)
    {
        // try get color from block definition
        // else return missing block color
        string blockName = highestBlocks[blockCoordsRel.X, blockCoordsRel.Z].Name;
        if (definition.BlockDefinitions.TryGetValue(blockName, out ViewportBlockDefinition? bd))
            return bd.Color;
        else
            return definition.MissingBlockDefinition.Color;
    }
}
