namespace binstarjs03.AerialOBJ.WpfApp.Services.ChunkRegionManaging;

public class LinearChunkLoadingPattern : IChunkLoadingPattern
{
    public string PatternName => "Linear";
    public int GetPendingChunkIndex(int pendingChunkCount)
    {
        return 0;
    }
}
