using System.Threading;

using binstarjs03.AerialOBJ.Core.MinecraftWorldRefactor;
using binstarjs03.AerialOBJ.Core.Primitives;
using binstarjs03.AerialOBJ.WpfApp.Components;
using binstarjs03.AerialOBJ.WpfApp.Models;

namespace binstarjs03.AerialOBJ.WpfApp.Services.ChunkRendering;
/// <summary>
/// Abstraction for chunk shading that controls how blocks will be rendered
/// </summary>
public interface IChunkShader
{
    void RenderChunk(IRegionImage regionImage, Block[,] highestBlocks, Point2Z<int> chunkCoordsRel);
}
