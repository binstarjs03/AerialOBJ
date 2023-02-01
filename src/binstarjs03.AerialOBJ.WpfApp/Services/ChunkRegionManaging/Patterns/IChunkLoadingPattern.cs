namespace binstarjs03.AerialOBJ.WpfApp.Services.ChunkRegionManaging;
public interface IChunkLoadingPattern
{
    string PatternName { get; }
    int GetPendingChunkIndex(int pendingChunkCount);
}