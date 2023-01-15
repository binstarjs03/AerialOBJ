using System;

using binstarjs03.AerialOBJ.Core;
using binstarjs03.AerialOBJ.Core.MinecraftWorld;
using binstarjs03.AerialOBJ.Core.Primitives;
using binstarjs03.AerialOBJ.WpfApp.Components;

namespace binstarjs03.AerialOBJ.WpfApp.Services.ChunkRendering;
public class ChunkRenderer : IChunkRenderer
{
    private IChunkShader _shader;
    private readonly IDefinitionManagerService _definitionManager;
    private readonly Color _transparent = new() { Alpha = 0, Red = 0, Green = 0, Blue = 0 };

    public ChunkRenderer(IChunkShader initialChunkShader, IDefinitionManagerService definitionManager)
    {
        _shader = initialChunkShader;
        _definitionManager = definitionManager;
    }

    // TODO lock chunkShader if there are reading threads
    // TODO make chunkshader swappable and refresh CRM just like swapping definition
    public IChunkShader Shader
    {
        get => _shader;
        set => throw new NotImplementedException();
    }

    public void RenderChunk(IRegionImage regionImage, Block[,] highestBlocks, Point2Z<int> chunkCoordsRel)
    {
        Shader.RenderChunk(_definitionManager.CurrentViewportDefinition, regionImage, highestBlocks, chunkCoordsRel);
    }

    public void EraseChunk(IRegionImage regionImage, Point2Z<int> chunkCoordsRel)
    {
        for (int x = 0; x < IChunk.BlockCount; x++)
            for (int z = 0; z < IChunk.BlockCount; z++)
            {
                Point2Z<int> blockCoordsRel = new(x, z);
                Point2<int> pixelCoords = ChunkRenderMath.GetRegionImagePixelCoords(chunkCoordsRel, blockCoordsRel);
                regionImage[pixelCoords.X, pixelCoords.Y] = _transparent;
            }
    }

    public void RenderRandomNoise(IRegionImage regionImage, Color color, byte distance)
    {
        for (int x = 0; x < regionImage.Size.Width; x++)
            for (int y = 0; y < regionImage.Size.Height; y++)
                regionImage[x, y] = Random.Shared.NextColor(color, distance);
    }
}
