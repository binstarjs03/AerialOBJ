namespace binstarjs03.AerialOBJ.Core.Primitives;

public readonly struct ChunkSectorTableEntryData
{
    public int SectorPos { get; }
    public int SectorSize { get; }

    public ChunkSectorTableEntryData(int sectorPos, int sectorSize)
    {
        SectorPos = sectorPos;
        SectorSize = sectorSize;
    }
}
