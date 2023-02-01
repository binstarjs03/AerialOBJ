using System;

namespace binstarjs03.AerialOBJ.WpfApp.Services.ChunkLoadingPatterns;
public class AlternateCheckerboardChunkLoadingPattern : IChunkLoadingPattern
{
    private int _counter = 1;

    public string PatternName => "Alternate Checkerboard";

    public int GetPendingChunkIndex(int pendingChunkCount)
    {
        int index = Math.Min(_counter *= 3, pendingChunkCount - 1);
        if (_counter >= 1000 || _counter >= pendingChunkCount)
            _counter = 1;
        return index;
    }
}