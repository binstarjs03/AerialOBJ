﻿namespace binstarjs03.AerialOBJ.MvvmAppCore.Services.ChunkLoadingPatterns;
public class LinearChunkLoadingPattern : IChunkLoadingPattern
{
    public string PatternName => "Linear";
    public int GetPendingChunkIndex(int pendingChunkCount)
    {
        return 0;
    }
}
