namespace binstarjs03.AerialOBJ.Core.MinecraftWorldRefactor;

public class ChunkHighestBlockBuffer
{
    public string[,] Names { get; } = new string[IChunk.BlockCount, IChunk.BlockCount];
    public int[,] Heights { get; } = new int[IChunk.BlockCount, IChunk.BlockCount];
}
