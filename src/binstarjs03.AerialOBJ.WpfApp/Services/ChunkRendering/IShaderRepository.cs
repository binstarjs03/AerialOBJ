using System.Collections.Generic;

using binstarjs03.AerialOBJ.Imaging.ChunkRendering;

namespace binstarjs03.AerialOBJ.WpfApp.Services.ChunkRendering;
public interface IChunkShaderRepository
{
    Dictionary<string, IChunkShader> Shaders { get; }
    IChunkShader DefaultShader { get; }
}
