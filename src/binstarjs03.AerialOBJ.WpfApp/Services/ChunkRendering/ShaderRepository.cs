using System.Collections.Generic;

using binstarjs03.AerialOBJ.Imaging.ChunkRendering;

namespace binstarjs03.AerialOBJ.WpfApp.Services.ChunkRendering;

public interface IShaderRepository
{
    Dictionary<string, IChunkShader> Shaders { get; }
    IChunkShader DefaultShader { get; }
}

public class ShaderRepository : IShaderRepository
{
    public ShaderRepository(Dictionary<string, IChunkShader> shaders, IChunkShader defaultShader)
    {
        Shaders = shaders;
        DefaultShader = defaultShader;
    }

    public Dictionary<string, IChunkShader> Shaders { get; }
    public IChunkShader DefaultShader { get; }
}
