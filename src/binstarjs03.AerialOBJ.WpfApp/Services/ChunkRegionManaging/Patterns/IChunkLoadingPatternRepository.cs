using System.Collections.Generic;

namespace binstarjs03.AerialOBJ.WpfApp.Services.ChunkRegionManaging.Patterns;
public interface IChunkLoadingPatternRepository
{
    Dictionary<string, IChunkLoadingPattern> ChunkLoadingPatternDict { get; }
    IChunkLoadingPattern DefaultChunkLoadingPattern { get; }
    List<IChunkLoadingPattern> ChunkLoadingPatternList { get; }
}

public class ChunkLoadingPatternRepository : IChunkLoadingPatternRepository
{

    public ChunkLoadingPatternRepository(Dictionary<string, IChunkLoadingPattern> chunkLoadingPatternDict, IChunkLoadingPattern defaultChunkLoadingPattern)
    {
        ChunkLoadingPatternDict = chunkLoadingPatternDict;
        ChunkLoadingPatternList = new List<IChunkLoadingPattern>(chunkLoadingPatternDict.Values);
        DefaultChunkLoadingPattern = defaultChunkLoadingPattern;
    }

    public Dictionary<string, IChunkLoadingPattern> ChunkLoadingPatternDict { get; }
    public List<IChunkLoadingPattern> ChunkLoadingPatternList { get; }
    public IChunkLoadingPattern DefaultChunkLoadingPattern { get; }
}