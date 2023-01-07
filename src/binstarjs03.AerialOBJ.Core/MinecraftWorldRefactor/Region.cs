using System;
using System.Buffers.Binary;
using System.IO;

using binstarjs03.AerialOBJ.Core.Nbt;
using binstarjs03.AerialOBJ.Core.Primitives;

using CoordsConversion = binstarjs03.AerialOBJ.Core.MathUtils.MinecraftCoordsConversion;

namespace binstarjs03.AerialOBJ.Core.MinecraftWorldRefactor;
public class Region
{
    public const int BlockCount = ChunkCount * IChunk.BlockCount;

    public const int ChunkCount = 32;
    public const int TotalChunkCount = ChunkCount * ChunkCount;
    public const int ChunkRange = ChunkCount - 1;
    public static readonly Point2ZRange<int> ChunkRangeRel = new(Point2Z<int>.Zero, new Point2Z<int>(ChunkRange, ChunkRange));

    public const int SectorDataLength = 4096;
    public const int ChunkSectorTableSize = SectorDataLength;
    public const int ChunkSectorTableEntrySize = 4;

    private readonly byte[] _data;

    public Region(byte[] data, Point2Z<int> regionCoords)
    {
        verifyDataLength(data.Length);
        _data = data;
        Coords = regionCoords;
        ChunkRangeAbs = CoordsConversion.CalculateChunkRangeAbsForRegion(regionCoords);

        static void verifyDataLength(long dataLength)
        {
            if (dataLength == 0)
                throw new RegionNoDataException();
            if (dataLength < ChunkSectorTableSize)
                throw new InvalidDataException("Region data is too small");
        }
    }

    public Point2Z<int> Coords { get; }
    public Point2ZRange<int> ChunkRangeAbs { get; }

    public IChunk GetChunk(Point2Z<int> chunkCoordsRel)
    {
        ChunkRangeRel.ThrowIfOutside(chunkCoordsRel);
        if (!HasChunkGenerated(chunkCoordsRel))
            throw new ChunkNotGeneratedException($"Chunk {chunkCoordsRel} (relative) is not generated yet");

        ChunkSectorTableEntry cste = GetChunkSectorTableEntry(chunkCoordsRel);
        (int chunkDataStartPos, int chunkDataLength) = getChunkNbtData(cste);

        using MemoryStream chunkNbtStream = new(_data, chunkDataStartPos, chunkDataLength);
        NbtCompound chunkNbt = (NbtIO.ReadStream(chunkNbtStream) as NbtCompound)!;
        return ChunkFactory.CreateInstance(chunkNbt);

        (int dataStartPos, int dataLength) getChunkNbtData(ChunkSectorTableEntry cste)
        {
            int startSectorData = cste.SectorPos * SectorDataLength;
            int chunkNbtLength = BinaryPrimitives.ReadInt32BigEndian(Read(startSectorData, 4)) - 1;
            int chunkNbtDataStart = startSectorData + 5;
            return (chunkNbtDataStart, chunkNbtLength);
        }
    }

    public bool HasChunkGenerated(Point2Z<int> chunkCoordsRel)
    {
        ChunkSectorTableEntry cste = GetChunkSectorTableEntry(chunkCoordsRel);
        return cste.SectorPos != 0 && cste.SectorSize != 0;
    }

    private Span<byte> Read(int pos, int length) => new(_data, pos, length);

    private ChunkSectorTableEntry GetChunkSectorTableEntry(Point2Z<int> chunkCoordsRel)
    {
        ChunkRangeRel.ThrowIfOutside(chunkCoordsRel);
        int startData = (chunkCoordsRel.X + chunkCoordsRel.Z * ChunkCount) * ChunkSectorTableEntrySize;
        Span<byte> tableEntryData = Read(startData, ChunkSectorTableEntrySize);
        int sectorPos = 0;
        // non human-friendly converting arbitary bytes into integer
        for (int i = 0; i < 3; i++)
        {
            int buff = tableEntryData[i];
            buff <<= (3 - i - 1) * 8;
            sectorPos += buff;
        }
        int sectorSize = tableEntryData[3];
        return new ChunkSectorTableEntry
        {
            SectorPos = sectorPos,
            SectorSize = sectorSize,
        };
    }

    public override string ToString() => $"Region {Coords}";
}
