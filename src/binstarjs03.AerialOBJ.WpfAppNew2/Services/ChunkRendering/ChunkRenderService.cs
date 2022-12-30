using System;
using System.Threading;
using System.Windows.Threading;

using binstarjs03.AerialOBJ.Core;
using binstarjs03.AerialOBJ.Core.MinecraftWorld;
using binstarjs03.AerialOBJ.Core.Primitives;
using binstarjs03.AerialOBJ.WpfAppNew2.Components;
using binstarjs03.AerialOBJ.WpfAppNew2.Models;

namespace binstarjs03.AerialOBJ.WpfAppNew2.Services.ChunkRendering;
public class ChunkRenderService : IChunkRenderService
{
    private IChunkShader _chunkShader;
    private readonly Color _transparent = new() { Alpha = 0, Red = 0, Green = 0, Blue = 0 };

    public ChunkRenderService(IChunkShader initialChunkShader)
    {
        _chunkShader = initialChunkShader;
    }

    public void RenderChunk(RegionModel region, Chunk chunk, CancellationToken cancellationToken)
    {
        _chunkShader.RenderChunk(region, chunk, cancellationToken);
    }

    public void EraseChunk(RegionModel region, Chunk chunk, CancellationToken cancellationToken)
    {
        for (int x = 0; x < Section.BlockCount; x++)
            for (int z = 0; z < Section.BlockCount; z++)
            {
                Point2Z<int> blockCoordsRel = new(x, z);
                Point2<int> pixelCoords = ChunkRenderMath.GetRegionImagePixelCoords(chunk.ChunkCoordsRel, blockCoordsRel);
                region.RegionImage[pixelCoords.X, pixelCoords.Y] = _transparent;
            }
        App.Current.Dispatcher.InvokeAsync(region.RegionImage.Redraw, DispatcherPriority.Background, cancellationToken);
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
