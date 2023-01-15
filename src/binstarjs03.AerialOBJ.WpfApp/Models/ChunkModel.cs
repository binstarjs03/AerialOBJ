using System;

using binstarjs03.AerialOBJ.Core.MinecraftWorld;
using binstarjs03.AerialOBJ.Core.Pooling;

namespace binstarjs03.AerialOBJ.WpfApp.Models;
public class ChunkModel : IDisposable
{
    private static readonly ArrayPool2<Block> s_highestBlockPooler = new(IChunk.BlockCount, IChunk.BlockCount);
    private bool _disposed;

    public required IChunk Data { get; init; }

    // TODO maybe we should refactor it to not hold buffer unless if we really need it
    // This way, memory usage is reduced because there are only handful of blocks pool object
    // instead of more than 10K (loaded chunks) buffers are being held
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
