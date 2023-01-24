using System;
using System.Collections.Generic;

using binstarjs03.AerialOBJ.Core;
using binstarjs03.AerialOBJ.Core.MinecraftWorld;
using binstarjs03.AerialOBJ.Core.Primitives;
using binstarjs03.AerialOBJ.Imaging.ChunkRendering;
using binstarjs03.AerialOBJ.WpfApp.Components;

namespace binstarjs03.AerialOBJ.WpfApp.Services.ChunkRendering;
public class ChunkRenderer : IChunkRenderer
{
    private IChunkShader _shader;
    private readonly IDefinitionManager _definitionManager;
    private readonly Color _transparent = new() { Alpha = 0, Red = 0, Green = 0, Blue = 0 };

    public ChunkRenderer(IChunkShader initialChunkShader, IDefinitionManager definitionManager)
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

    public void RenderChunk(IRegionImage regionImage, IChunk chunk, BlockSlim[,] highestBlocks, int heightLimit)
    {
        ChunkRenderSetting setting = new()
        {
            ViewportDefinition = _definitionManager.CurrentViewportDefinition,
            Image = regionImage,
            RenderPosition = new PointY<int>(0,0),
            HeightLimit = heightLimit,
            Chunk = chunk,
            HighestBlocks = highestBlocks,
            Exclusions = null, // not implemented for now
        };
        Shader.RenderChunk(setting);
    }

    public void EraseChunk(IRegionImage regionImage, PointZ<int> chunkCoordsRel)
    {
        for (int x = 0; x < IChunk.BlockCount; x++)
            for (int z = 0; z < IChunk.BlockCount; z++)
            {
                // use chunk coords as rendering position also as the chunk coords rel too
                PointZ<int> blockCoordsRel = new(x, z);
                PointY<int> pixelCoords = IChunkShader.GetPixelCoordsForBlock(chunkCoordsRel, chunkCoordsRel, blockCoordsRel);
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
