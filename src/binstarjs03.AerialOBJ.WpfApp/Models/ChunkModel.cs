using System;

using binstarjs03.AerialOBJ.Core.MinecraftWorld;
using binstarjs03.AerialOBJ.Core.Pooling;
using binstarjs03.AerialOBJ.Core.Primitives;

namespace binstarjs03.AerialOBJ.WpfApp.Models;
public class ChunkModel : IDisposable
{
    private static readonly ArrayPool2<BlockSlim> s_highestBlockPooler = new(IChunk.BlockCount, IChunk.BlockCount);
    private bool _disposed;

    //public required IChunk Data { get; init; }
    public ChunkModel(Point2Z<int> coordsAbs, Point2Z<int> coordsRel)
    {
        CoordsAbs = coordsAbs;
        CoordsRel = coordsRel;
    }

    public Point2Z<int> CoordsAbs { get; }
    public Point2Z<int> CoordsRel { get; }

    // TODO maybe we should refactor it to not hold buffer unless if we really need it
    // This way, memory usage is reduced because there are only handful of blocks pool object
    // instead of more than 10K (loaded chunks) buffers are being held
    public BlockSlim[,] HighestBlocks { get; } = s_highestBlockPooler.Rent();

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
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
