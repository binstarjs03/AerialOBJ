using System;

namespace binstarjs03.AerialOBJ.WpfApp.Services.ChunkLoadingPatterns;
public class RandomChunkLoadingPattern : IChunkLoadingPattern
{
    private readonly Random _random = new();

    public string PatternName => "Random";

    public int GetPendingChunkIndex(int pendingChunkCount)
    {
        return _random.Next(pendingChunkCount);
    }
}
