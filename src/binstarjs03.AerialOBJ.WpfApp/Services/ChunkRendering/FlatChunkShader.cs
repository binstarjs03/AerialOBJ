using binstarjs03.AerialOBJ.Core.Definitions;
using binstarjs03.AerialOBJ.Core.MinecraftWorld;
using binstarjs03.AerialOBJ.Core.Primitives;
using binstarjs03.AerialOBJ.WpfApp.Components;

namespace binstarjs03.AerialOBJ.WpfApp.Services.ChunkRendering;
/// <summary>
/// Chunk Shader that render blocks in a flat solid color
/// </summary>
public class FlatChunkShader : IChunkShader
{
    private readonly IDefinitionManagerService _definitionManager;

    public FlatChunkShader(IDefinitionManagerService definitionManager)
    {
        _definitionManager = definitionManager;
    }

    public void RenderChunk(IRegionImage regionImage, Block[,] highestBlocks, Point2Z<int> chunkCoordsRel)
    {
        for (int x = 0; x < IChunk.BlockCount; x++)
            for (int z = 0; z < IChunk.BlockCount; z++)
            {
                Point2Z<int> blockCoordsRel = new(x, z);
                Point2<int> pixelCoords = ChunkRenderMath.GetRegionImagePixelCoords(chunkCoordsRel, blockCoordsRel);
                Color color = GetBlockColor(highestBlocks, blockCoordsRel);
                regionImage[pixelCoords.X, pixelCoords.Y] = color;
            }
    }

    private Color GetBlockColor(Block[,] highestBlocks, Point2Z<int> blockCoordsRel)
    {
        // try get color from block definition
        // else return missing block color
        string blockName = highestBlocks[blockCoordsRel.X, blockCoordsRel.Z].Name;
        if (_definitionManager.CurrentViewportDefinition.BlockDefinitions.TryGetValue(blockName, out ViewportBlockDefinition? bd))
            return bd.Color;
        else
            return _definitionManager.CurrentViewportDefinition.MissingBlockDefinition.Color;
    }
}
