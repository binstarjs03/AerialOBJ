using binstarjs03.AerialOBJ.Core.Definitions;
using binstarjs03.AerialOBJ.Core.MinecraftWorld;
using binstarjs03.AerialOBJ.Core.Primitives;
using binstarjs03.AerialOBJ.WpfApp.Components;

namespace binstarjs03.AerialOBJ.WpfApp.Services.ChunkRendering;
/// <summary>
/// Abstraction for chunk shading that controls how blocks will be rendered
/// </summary>
public interface IChunkShader
{
    void RenderChunk(ViewportDefinition vd, IRegionImage regionImage, BlockSlim[,] highestBlocks, PointZ<int> chunkCoordsRel);
}
