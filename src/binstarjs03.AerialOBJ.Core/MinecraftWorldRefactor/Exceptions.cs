using System;

namespace binstarjs03.AerialOBJ.Core.MinecraftWorldRefactor;

public class ChunkNotGeneratedException : InvalidOperationException
{
    public ChunkNotGeneratedException() : base("Chunk is not generated yet") { }
    public ChunkNotGeneratedException(string message) : base(message) { }
}

public class RegionNoDataException : Exception
{
    public RegionNoDataException() { }

    public RegionNoDataException(string? message) : base(message) { }

    public RegionNoDataException(string? message, Exception? innerException) : base(message, innerException) { }
}