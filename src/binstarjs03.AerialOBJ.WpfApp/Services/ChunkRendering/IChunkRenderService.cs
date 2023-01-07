using binstarjs03.AerialOBJ.Core.MinecraftWorld;
using binstarjs03.AerialOBJ.Core.Primitives;
using binstarjs03.AerialOBJ.WpfApp.Components;

namespace binstarjs03.AerialOBJ.WpfApp.Services.ChunkRendering;
public interface IChunkRenderService
{
    IChunkShader Shader { get; set; }
    void RenderRandomNoise(IRegionImage regionImage, Color color, byte distance);
    void RenderChunk(IRegionImage regionImage, Block[,] highestBlocks, Point2Z<int> chunkCoordsRel);
    void EraseChunk(IRegionImage regionImage, Point2Z<int> chunkCoordsRel);
}
