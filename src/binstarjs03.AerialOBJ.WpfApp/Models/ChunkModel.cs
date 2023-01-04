using System;

using binstarjs03.AerialOBJ.Core.ArrayPooling;
using binstarjs03.AerialOBJ.Core.MinecraftWorldRefactor;

namespace binstarjs03.AerialOBJ.WpfApp.Models;
public class ChunkModel : IDisposable
{
    private static readonly ArrayPool2<Block> _highestBlockPooler = new(IChunk.BlockCount, IChunk.BlockCount);
    private bool _disposed;

    public required IChunk ChunkData { get; init; }
    public Block[,] HighestBlocks { get; } = _highestBlockPooler.Rent();

    public void LoadHighestBlock() => ChunkData.GetHighestBlock(HighestBlocks);

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                ChunkData.Dispose();
                _highestBlockPooler.Return(HighestBlocks);
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
