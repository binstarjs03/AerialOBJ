using System;

using binstarjs03.AerialOBJ.Core;
using binstarjs03.AerialOBJ.Core.Primitives;
using binstarjs03.AerialOBJ.WpfAppNew2.Components;

namespace binstarjs03.AerialOBJ.WpfAppNew2.Services;
public class ChunkRenderService : IChunkRenderService
{
    public void RenderRandomNoise(IMutableImage mutableImage, Color color, byte distance)
    {
        for (int x = 0; x < mutableImage.Size.Width; x++)
            for (int y = 0; y < mutableImage.Size.Height; y++)
                mutableImage[x, y] = Random.Shared.NextColor(color, distance);
    }
}
