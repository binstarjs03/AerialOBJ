using System;

namespace binstarjs03.AerialOBJ.WpfApp.Services.ChunkRegionManaging;

public class SplitChunkLoadingPattern : IChunkLoadingPattern
{
    public int GetPendingChunkIndex(int pendingChunkCount)
    {
        return Math.Min(pendingChunkCount / 2, pendingChunkCount - 1);
    }
}