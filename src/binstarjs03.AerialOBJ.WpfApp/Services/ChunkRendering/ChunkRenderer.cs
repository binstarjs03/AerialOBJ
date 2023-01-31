using System;

using binstarjs03.AerialOBJ.Core;
using binstarjs03.AerialOBJ.Core.MinecraftWorld;
using binstarjs03.AerialOBJ.Core.Primitives;
using binstarjs03.AerialOBJ.Imaging.ChunkRendering;
using binstarjs03.AerialOBJ.WpfApp.Components;
using binstarjs03.AerialOBJ.WpfApp.Settings;

namespace binstarjs03.AerialOBJ.WpfApp.Services.ChunkRendering;
public class ChunkRenderer : IChunkRenderer
{
    private readonly DefinitionSetting _definitionSetting;
    private readonly ViewportSetting _viewportSetting;
    private readonly Color _transparent = new() { Alpha = 0, Red = 0, Green = 0, Blue = 0 };

    public ChunkRenderer(DefinitionSetting definitionSetting, ViewportSetting viewportSetting)
    {
        _definitionSetting = definitionSetting;
        _viewportSetting = viewportSetting;
    }

    public void RenderChunk(IRegionImage regionImage, IChunk chunk, BlockSlim[,] highestBlocks, int heightLimit)
    {
        ChunkRenderOptions renderOptions = new()
        {
            ViewportDefinition = _definitionSetting.CurrentViewportDefinition,
            Image = regionImage,
            RenderPosition = new PointY<int>(0, 0),
            HeightLimit = heightLimit,
            Chunk = chunk,
            HighestBlocks = highestBlocks,
            Exclusions = null, // not implemented for now
        };
        _viewportSetting.ChunkShader.RenderChunk(renderOptions);
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
