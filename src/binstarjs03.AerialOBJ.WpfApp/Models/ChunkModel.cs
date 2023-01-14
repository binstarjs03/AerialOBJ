using System;

using binstarjs03.AerialOBJ.Core.MinecraftWorld;
using binstarjs03.AerialOBJ.Core.Pooling;

namespace binstarjs03.AerialOBJ.WpfApp.Models;
public class ChunkModel : IDisposable
{
    private static readonly ArrayPool2<Block> s_highestBlockPooler = new(IChunk.BlockCount, IChunk.BlockCount);
    private bool _disposed;

    public required IChunk Data { get; init; }
    public Block[,] HighestBlocks { get; } = s_highestBlockPooler.Rent();

    public void LoadHighestBlock(int heightLimit) => Data.GetHighestBlock(HighestBlocks, heightLimit);

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                Data.Dispose();
                s_highestBlockPooler.Return(HighestBlocks);
            }
            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
