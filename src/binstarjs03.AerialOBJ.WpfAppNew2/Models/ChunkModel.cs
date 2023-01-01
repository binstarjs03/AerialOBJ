using binstarjs03.AerialOBJ.Core.MinecraftWorld;

namespace binstarjs03.AerialOBJ.WpfAppNew2.Models;
public class ChunkModel
{
    public required Chunk ChunkData { get; set; }
    public ChunkHighestBlockInfo HighestBlock { get; set; } = new();
    public void LoadHighestBlock()
    {
        ChunkData.GetHighestBlock(HighestBlock);
    }
}
