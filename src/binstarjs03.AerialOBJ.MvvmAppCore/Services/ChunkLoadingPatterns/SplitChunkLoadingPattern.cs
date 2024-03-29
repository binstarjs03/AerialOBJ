﻿using System;

namespace binstarjs03.AerialOBJ.MvvmAppCore.Services.ChunkLoadingPatterns;
public class SplitChunkLoadingPattern : IChunkLoadingPattern
{
    public string PatternName => "Split";

    public int GetPendingChunkIndex(int pendingChunkCount)
    {
        return Math.Min(pendingChunkCount / 2, pendingChunkCount - 1);
    }
}