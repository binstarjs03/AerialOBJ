using System.Windows.Threading;

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

    public void RenderChunk(RegionModel regionModel, Chunk chunk)
    {
        ChunkHighestBlockInfo highestBlockInfo = new();
        chunk.GetHighestBlock(highestBlockInfo);
        for (int x = 0; x < Section.BlockCount; x++)
            for (int z = 0; z < Section.BlockCount; z++)
            {
                Color color = ...; // find color in block definition
                Point2<int> pixelPos = new(chunk.ChunkCoordsRel.X * Section.BlockCount + x,
                                           chunk.ChunkCoordsRel.X * Section.BlockCount + z);
                regionModel.RegionImage[pixelPos.X, pixelPos.Y] = color;
            }
        App.Current.Dispatcher.InvokeAsync(regionModel.RegionImage.Redraw, DispatcherPriority.Background);
    }


}
