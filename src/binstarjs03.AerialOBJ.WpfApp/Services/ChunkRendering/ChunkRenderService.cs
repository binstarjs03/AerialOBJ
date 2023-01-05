using System;
using System.Threading;

using binstarjs03.AerialOBJ.Core;
using binstarjs03.AerialOBJ.Core.MinecraftWorldRefactor;
using binstarjs03.AerialOBJ.Core.Primitives;
using binstarjs03.AerialOBJ.WpfApp.Components;
using binstarjs03.AerialOBJ.WpfApp.Models;

namespace binstarjs03.AerialOBJ.WpfApp.Services.ChunkRendering;
public class ChunkRenderService : IChunkRenderService
{
    private IChunkShader _chunkShader;
    private readonly Color _transparent = new() { Alpha = 0, Red = 0, Green = 0, Blue = 0 };

    public ChunkRenderService(IChunkShader initialChunkShader)
    {
        _chunkShader = initialChunkShader;
    }

    // TODO pass in an interface of IRegionImage instead
    public void RenderChunk(IRegionImage regionImage, Block[,] highestBlocks, Point2Z<int> chunkCoordsRel)
    {
        _chunkShader.RenderChunk(regionImage, highestBlocks, chunkCoordsRel);
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

    // TODO lock chunkShader if there are reading threads
    public void SetShader(IChunkShader shader)
    {
        _chunkShader = shader;
    }
}
