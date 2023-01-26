﻿using System;

namespace binstarjs03.AerialOBJ.WpfApp.Services.ChunkRegionManaging;

public class RandomChunkLoadingPattern : IChunkLoadingPattern
{
    private readonly Random _random = new();

    public int GetPendingChunkIndex(int pendingChunkCount)
    {
        return _random.Next(pendingChunkCount);
    }
}