using System;
using System.IO;

namespace binstarjs03.AerialOBJ.Core.WorldRegion;

public class ChunkNotGeneratedException : InvalidOperationException
{
    public ChunkNotGeneratedException() : base("Chunk is not generated yet") { }
    public ChunkNotGeneratedException(string message) : base(message) { }
}

// provide file information if neccessary, maybe even derive from ioexception
public class RegionUnrecognizedFileException : Exception
{
    public RegionUnrecognizedFileException() : base("Cannot open File as region file") { }
    public RegionUnrecognizedFileException(string message) : base(message) { }
}