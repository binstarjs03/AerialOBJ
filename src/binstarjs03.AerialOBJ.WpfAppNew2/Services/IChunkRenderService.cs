using binstarjs03.AerialOBJ.Core.Primitives;
using binstarjs03.AerialOBJ.WpfAppNew2.Components;

namespace binstarjs03.AerialOBJ.WpfAppNew2.Services;
public interface IChunkRenderService
{
    void RenderRandomNoise(IMutableImage mutableImage, Color color, byte distance);
}
