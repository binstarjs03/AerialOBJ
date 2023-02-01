using binstarjs03.AerialOBJ.Core.MinecraftWorld;
using binstarjs03.AerialOBJ.Core.Primitives;
using binstarjs03.AerialOBJ.WpfApp.Components;

namespace binstarjs03.AerialOBJ.WpfApp.Services;
public interface IChunkRenderer
{
    void RenderRandomNoise(IRegionImage regionImage, Color color, byte distance);
    void RenderChunk(IRegionImage regionImage, IChunk chunk, BlockSlim[,] highestBlocks, int heightLimit);
    void EraseChunk(IRegionImage regionImage, PointZ<int> chunkCoordsRel);
}
