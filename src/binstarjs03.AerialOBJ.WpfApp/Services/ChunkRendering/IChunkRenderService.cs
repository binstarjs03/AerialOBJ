using System.Threading;

using binstarjs03.AerialOBJ.Core.MinecraftWorldRefactor;
using binstarjs03.AerialOBJ.Core.Primitives;
using binstarjs03.AerialOBJ.WpfApp.Components;
using binstarjs03.AerialOBJ.WpfApp.Models;

namespace binstarjs03.AerialOBJ.WpfApp.Services.ChunkRendering;
public interface IChunkRenderService
{
    void RenderRandomNoise(IMutableImage mutableImage, Color color, byte distance);
    void RenderChunk(RegionModel regionModel, Block[,] highestBlocks, Point2Z<int> chunkCoordsRel);
    void EraseChunk(RegionModel regionModel, Point2Z<int> chunkCoordsRel);
    void SetShader(IChunkShader shader);
}
