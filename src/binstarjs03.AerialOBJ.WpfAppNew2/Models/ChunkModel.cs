using binstarjs03.AerialOBJ.Core.MinecraftWorldRefactor;

namespace binstarjs03.AerialOBJ.WpfAppNew2.Models;
public class ChunkModel
{
    public required IChunk ChunkData { get; set; }
    public ChunkHighestBlockBuffer HighestBlock { get; set; } = new();
    public void LoadHighestBlock()
    {
        ChunkData.GetHighestBlock(HighestBlock);
    }
}
