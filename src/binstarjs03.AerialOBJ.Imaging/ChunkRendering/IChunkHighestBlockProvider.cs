using binstarjs03.AerialOBJ.Core.MinecraftWorld;
using binstarjs03.AerialOBJ.Core.Primitives;

namespace binstarjs03.AerialOBJ.Imaging.ChunkRendering;

public interface IChunkHighestBlockProvider
{
    BlockSlim[,] GetChunkHighestBlocks(PointZ<int> chunkCoords);
}