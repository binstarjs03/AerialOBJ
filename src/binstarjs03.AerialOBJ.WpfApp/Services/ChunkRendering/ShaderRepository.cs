using System.Collections.Generic;

using binstarjs03.AerialOBJ.Imaging.ChunkRendering;

namespace binstarjs03.AerialOBJ.WpfApp.Services.ChunkRendering;
public class ChunkShaderRepository : IChunkShaderRepository
{
    public ChunkShaderRepository(Dictionary<string, IChunkShader> shaders, IChunkShader defaultShader)
    {
        ShaderDict = shaders;
        ShaderList = new List<IChunkShader>(shaders.Values);
        DefaultShader = defaultShader;
    }

    public Dictionary<string, IChunkShader> ShaderDict { get; }
    public List<IChunkShader> ShaderList { get; }
    public IChunkShader DefaultShader { get; }
}
