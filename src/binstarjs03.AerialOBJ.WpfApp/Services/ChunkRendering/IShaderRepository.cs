using System.Collections.Generic;

using binstarjs03.AerialOBJ.Imaging.ChunkRendering;

namespace binstarjs03.AerialOBJ.WpfApp.Services.ChunkRendering;
public interface IChunkShaderRepository
{
    Dictionary<string, IChunkShader> ShaderDict { get; }
    IChunkShader DefaultShader { get; }
    List<IChunkShader> ShaderList { get; }
}
