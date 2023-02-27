namespace binstarjs03.AerialOBJ.MvvmAppCore.Services.ChunkLoadingPatterns;
public interface IChunkLoadingPattern
{
    string PatternName { get; }
    int GetPendingChunkIndex(int pendingChunkCount);
}