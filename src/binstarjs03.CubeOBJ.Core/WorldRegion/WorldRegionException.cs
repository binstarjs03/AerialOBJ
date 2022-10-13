using System;

namespace binstarjs03.AerialOBJ.Core.WorldRegion;

public class ChunkNotGeneratedException : InvalidOperationException
{
    public ChunkNotGeneratedException() : base("Chunk is not generated yet") { }
    public ChunkNotGeneratedException(string message) : base(message) { }
}
