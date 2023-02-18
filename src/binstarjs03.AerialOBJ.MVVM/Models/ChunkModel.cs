using System;

using binstarjs03.AerialOBJ.Core.MinecraftWorld;
using binstarjs03.AerialOBJ.Core.Pooling;
using binstarjs03.AerialOBJ.Core.Primitives;

namespace binstarjs03.AerialOBJ.MVVM.Models;
public class ChunkModel : IDisposable
{
    private static readonly ArrayPool2<BlockSlim> s_highestBlockPooler = new(IChunk.BlockCount, IChunk.BlockCount);
    private bool _disposed;

    public ChunkModel(PointZ<int> coordsAbs, PointZ<int> coordsRel)
    {
        CoordsAbs = coordsAbs;
        CoordsRel = coordsRel;
    }

    public PointZ<int> CoordsAbs { get; }
    public PointZ<int> CoordsRel { get; }

    public BlockSlim[,] HighestBlocks { get; } = s_highestBlockPooler.Rent();

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
                s_highestBlockPooler.Return(HighestBlocks);
            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
