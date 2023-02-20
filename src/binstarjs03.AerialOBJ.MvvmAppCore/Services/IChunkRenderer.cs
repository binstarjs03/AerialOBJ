using binstarjs03.AerialOBJ.Core.MinecraftWorld;
using binstarjs03.AerialOBJ.Core.Primitives;
using binstarjs03.AerialOBJ.MvvmAppCore.Components;

namespace binstarjs03.AerialOBJ.MvvmAppCore;

public interface IChunkRenderer
{
    void RenderChunk(IRegionImage regionImage, IChunk chunk, BlockSlim[,] highestBlocks, int heightLimit);
    void EraseChunk(IRegionImage regionImage, PointZ<int> chunkCoordsRel);
}