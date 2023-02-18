namespace binstarjs03.AerialOBJ.MVVM.Services.ChunkLoadingPatterns;
public interface IChunkLoadingPattern
{
    string PatternName { get; }
    int GetPendingChunkIndex(int pendingChunkCount);
}