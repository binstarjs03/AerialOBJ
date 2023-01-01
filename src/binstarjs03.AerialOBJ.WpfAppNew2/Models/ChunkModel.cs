using binstarjs03.AerialOBJ.Core.MinecraftWorldRefactor;

namespace binstarjs03.AerialOBJ.WpfAppNew2.Models;
public class ChunkModel
{
    public required IChunk ChunkData { get; init; }
    //public ChunkHighestBlockBuffer HighestBlock { get; set; } = new();
    public Block[,] HighestBlocks { get; init; } = new Block[IChunk.BlockCount, IChunk.BlockCount];
    public void LoadHighestBlock()
    {
        ChunkData.GetHighestBlock(HighestBlocks);
    }
}
