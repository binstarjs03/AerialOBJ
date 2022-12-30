using System.Threading;

using binstarjs03.AerialOBJ.Core.MinecraftWorld;
using binstarjs03.AerialOBJ.Core.Primitives;
using binstarjs03.AerialOBJ.WpfAppNew2.Models;

namespace binstarjs03.AerialOBJ.WpfAppNew2.Services.ChunkRendering;
/// <summary>
/// Abstraction for chunk shading that controls how blocks will be rendered
/// </summary>
public interface IChunkShader
{
    void RenderChunk(RegionModel regionModel, Chunk chunk, CancellationToken cancellationToken);
}
