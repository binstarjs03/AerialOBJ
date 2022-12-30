using System;

using binstarjs03.AerialOBJ.Core;
using binstarjs03.AerialOBJ.Core.MinecraftWorld;
using binstarjs03.AerialOBJ.Core.Primitives;
using binstarjs03.AerialOBJ.WpfAppNew2.Components;
using binstarjs03.AerialOBJ.WpfAppNew2.Models;

namespace binstarjs03.AerialOBJ.WpfAppNew2.Services.ChunkRendering;
public class ChunkRenderService : IChunkRenderService
{
    private IChunkShader _chunkShader;

    public ChunkRenderService(IChunkShader initialChunkShader)
    {
        _chunkShader = initialChunkShader;
    }


    public void RenderChunk(RegionModel region, Chunk chunk)
    {
        _chunkShader.RenderChunk(region, chunk);
    }

    public void RenderRandomNoise(IMutableImage mutableImage, Color color, byte distance)
    {
        for (int x = 0; x < mutableImage.Size.Width; x++)
            for (int y = 0; y < mutableImage.Size.Height; y++)
                mutableImage[x, y] = Random.Shared.NextColor(color, distance);
    }

    // TODO lock chunkShader if there are reading threads
    public void SetShader(IChunkShader shader)
    {
        _chunkShader = shader;
    }
}
