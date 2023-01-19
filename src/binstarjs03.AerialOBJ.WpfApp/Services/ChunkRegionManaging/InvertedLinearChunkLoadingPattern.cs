namespace binstarjs03.AerialOBJ.WpfApp.Services.ChunkRegionManaging;

public class InvertedLinearChunkLoadingPattern : IChunkLoadingPattern
{
    public int GetPendingChunkIndex(int pendingChunkCount)
    {
        return pendingChunkCount - 1;
    }
}
