using System.Threading;

using binstarjs03.AerialOBJ.Core.MinecraftWorldRefactor;
using binstarjs03.AerialOBJ.Core.Primitives;
using binstarjs03.AerialOBJ.WpfAppNew2.Components;
using binstarjs03.AerialOBJ.WpfAppNew2.Models;

namespace binstarjs03.AerialOBJ.WpfAppNew2.Services.ChunkRendering;
public interface IChunkRenderService
{
    void RenderRandomNoise(IMutableImage mutableImage, Color color, byte distance);
    void RenderChunk(RegionModel regionModel, ChunkHighestBlockBuffer highestBlocks, Point2Z<int> chunkCoordsRel, CancellationToken cancellationToken);
    void EraseChunk(RegionModel regionModel, ChunkModel chunkModel, CancellationToken cancellationToken);
    void SetShader(IChunkShader shader);
}
