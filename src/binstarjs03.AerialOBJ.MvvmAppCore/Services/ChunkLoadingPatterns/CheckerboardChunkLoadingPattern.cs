using System;

namespace binstarjs03.AerialOBJ.MvvmAppCore.Services.ChunkLoadingPatterns;
public class CheckerboardChunkLoadingPattern : IChunkLoadingPattern
{
    private int _counter = 1;

    public string PatternName => "Checkerboard";

    public int GetPendingChunkIndex(int pendingChunkCount)
    {
        int index = Math.Min(_counter++, pendingChunkCount - 1);
        if (_counter >= pendingChunkCount)
            _counter = 1;
        return index;
    }
}
