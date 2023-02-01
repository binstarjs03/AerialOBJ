namespace binstarjs03.AerialOBJ.WpfApp.Services.ChunkLoadingPatterns;
public interface IChunkLoadingPattern
{
    string PatternName { get; }
    int GetPendingChunkIndex(int pendingChunkCount);
}