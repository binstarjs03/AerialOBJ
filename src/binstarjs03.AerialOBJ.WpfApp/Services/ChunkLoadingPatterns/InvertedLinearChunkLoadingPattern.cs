namespace binstarjs03.AerialOBJ.WpfApp.Services.ChunkLoadingPatterns;
public class InvertedLinearChunkLoadingPattern : IChunkLoadingPattern
{
    public string PatternName => "Inverted Linear";

    public int GetPendingChunkIndex(int pendingChunkCount)
    {
        return pendingChunkCount - 1;
    }
}
