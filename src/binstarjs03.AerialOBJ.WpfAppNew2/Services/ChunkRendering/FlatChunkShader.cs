using System.Threading;
using System.Windows.Threading;

using binstarjs03.AerialOBJ.Core.Definitions;
using binstarjs03.AerialOBJ.Core.MinecraftWorld;
using binstarjs03.AerialOBJ.Core.Primitives;
using binstarjs03.AerialOBJ.WpfAppNew2.Models;

namespace binstarjs03.AerialOBJ.WpfAppNew2.Services.ChunkRendering;
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

    public void RenderChunk(RegionModel regionModel, ChunkHighestBlockInfo highestBlockInfo, Point2Z<int> chunkCoordsRel, CancellationToken cancellationToken)
    {
        for (int x = 0; x < Section.BlockCount; x++)
            for (int z = 0; z < Section.BlockCount; z++)
            {
                Point2Z<int> blockCoordsRel = new(x, z);
                Point2<int> pixelCoords = ChunkRenderMath.GetRegionImagePixelCoords(chunkCoordsRel, blockCoordsRel);
                Color color = GetBlockColor(_definitionManager.DefaultViewportDefinition, highestBlockInfo, blockCoordsRel);
                regionModel.RegionImage[pixelCoords.X, pixelCoords.Y] = color;
            }
        App.Current.Dispatcher.InvokeAsync(regionModel.RegionImage.Redraw, DispatcherPriority.Background, cancellationToken);
    }

    private static Color GetBlockColor(ViewportDefinition definition, ChunkHighestBlockInfo highestBlock, Point2Z<int> blockCoordsRel)
    {
        // try get color from block definition
        // else return missing block color
        string blockName = highestBlock.Names[blockCoordsRel.X, blockCoordsRel.Z];
        if (definition.BlockDefinitions.TryGetValue(blockName, out ViewportBlockDefinition? bd))
            return bd.Color;
        else
            return definition.MissingBlockDefinition.Color;
    }
}
