﻿namespace binstarjs03.AerialOBJ.Core.MinecraftWorldRefactor;

public readonly struct ChunkSectorTableEntry
{
    public required int SectorPos { get; init; }
    public required int SectorSize { get; init; }
}
