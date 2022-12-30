using binstarjs03.AerialOBJ.Core.MinecraftWorld;
using binstarjs03.AerialOBJ.Core.Primitives;
using binstarjs03.AerialOBJ.WpfAppNew2.Components;
using binstarjs03.AerialOBJ.WpfAppNew2.Models;

namespace binstarjs03.AerialOBJ.WpfAppNew2.Services.ChunkRendering;
public interface IChunkRenderService
{
    void RenderRandomNoise(IMutableImage mutableImage, Color color, byte distance);
    void RenderChunk(RegionModel region, Chunk chunk);
    void EraseChunk(RegionModel region, Chunk chunk);
    void SetShader(IChunkShader shader);
}
