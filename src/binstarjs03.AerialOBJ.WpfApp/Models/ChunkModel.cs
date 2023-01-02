using System;

using binstarjs03.AerialOBJ.Core.ArrayPooling;
using binstarjs03.AerialOBJ.Core.MinecraftWorldRefactor;

namespace binstarjs03.AerialOBJ.WpfApp.Models;
public class ChunkModel : IDisposable
{
    private static ArrayPool2<Block> _highestBlockPooler = new(IChunk.BlockCount, IChunk.BlockCount);
    private bool _disposedValue;

    public required IChunk ChunkData { get; init; }
    public Block[,] HighestBlocks { get; init; } = _highestBlockPooler.Rent();
    public void LoadHighestBlock()
    {
        ChunkData.GetHighestBlock(HighestBlocks);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                ChunkData.Dispose();
                _highestBlockPooler.Return(HighestBlocks);
            }
            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
