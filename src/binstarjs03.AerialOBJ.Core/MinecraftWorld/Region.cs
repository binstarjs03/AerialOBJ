/*
Copyright (c) 2022, Bintang Jakasurya
All rights reserved. 

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

using binstarjs03.AerialOBJ.Core.CoordinateSystem;
using binstarjs03.AerialOBJ.Core.Nbt;

namespace binstarjs03.AerialOBJ.Core.MinecraftWorld;

public class Region
{
    public const int BlockCount = Section.BlockCount * ChunkCount;
    public const int ChunkCount = 32;
    public const int TotalChunkCount = ChunkCount * ChunkCount;
    public const int ChunkRange = ChunkCount - 1;
    public static readonly CoordsRange2 ChunkRangeRel = new(
        min: Coords2.Zero,
        max: new Coords2(ChunkRange, ChunkRange)
    );

    public const int SectorDataSize = 4096;
    public const int ChunkHeaderTableSize = SectorDataSize * 1;
    public const int ChunkHeaderSize = 4;

    private readonly string _path;
    private readonly byte[] _data;
    private readonly Coords2 _regionCoords;
    private readonly CoordsRange2 _chunkRangeAbs;
    
    public Coords2 RegionCoords => _regionCoords;
    public CoordsRange2 ChunkRangeAbs => _chunkRangeAbs;

    public Region(string path, Coords2 regionCoords)
    {
        FileInfo fi = new(path);
        checkRegionData(fi);
        _path = path;
        _data = File.ReadAllBytes(path);
        _regionCoords = regionCoords;
        _chunkRangeAbs = calculateChunkRangeAbs(regionCoords);

        static void checkRegionData(FileInfo fileInfo)
        {
            if (fileInfo.Length > ChunkHeaderTableSize)
                return;
            string msg = "Region data is too small";
            throw new InvalidDataException(msg);
        }
        static CoordsRange2 calculateChunkRangeAbs(Coords2 regionCoords)
        {
            int chunkRangeAbsMinX = regionCoords.X * ChunkCount;
            int chunkRangeAbsMaxX = chunkRangeAbsMinX + ChunkRange;
            Range chunkRangeAbsX = new(chunkRangeAbsMinX, chunkRangeAbsMaxX);

            int chunkRangeAbsMinZ = regionCoords.Z * ChunkCount;
            int chunkRangeAbsMaxZ = chunkRangeAbsMinZ + ChunkRange;
            Range chunkRangeAbsZ = new(chunkRangeAbsMinZ, chunkRangeAbsMaxZ);

            return new CoordsRange2(chunkRangeAbsX, chunkRangeAbsZ);
        }
    }

    public static Region Open(string path, Coords2 coords)
        => new(path, coords);

    /// <exception cref="RegionUnrecognizedFileException"></exception>
    public static Region Open(string path)
    {
        FileInfo fi = new(path);
        string[] split = fi.Name.Split('.');
        bool correctPrefix = split[0] == "r";
        bool correctFileType = split[3] == "mca";
        bool validCoordinate = int.TryParse(split[1], out _) && int.TryParse(split[2], out _);
        if (correctPrefix && correctFileType && validCoordinate)
        {
            int x = int.Parse(split[1]);
            int z = int.Parse(split[2]);
            Coords2 coords = new(x, z);
            return new Region(path, coords);
        }
        else
            throw new RegionUnrecognizedFileException("Cannot automatically determine region position");
    }

    public static bool IsValidFilename(string regionFilename, out Coords2? regionCoords)
    {
        string[] split = regionFilename.Split('.');
        bool correctPrefix = split[0] == "r";
        bool correctFileType = split[3] == "mca";
        bool validX = int.TryParse(split[1], out int x);
        bool validZ = int.TryParse(split[2], out int z);
        bool validCoordinate = validX && validZ;
        if (correctPrefix && correctFileType && validCoordinate)
        {
            regionCoords = new Coords2(x, z);
            return true;
        }
        else
        {
            regionCoords = null;
            return false;
        }
    }

    public static Coords2 ConvertChunkCoordsAbsToRel(Coords2 coords)
    {
        int chunkCoordsRelX = MathUtils.Mod(coords.X, ChunkCount);
        int chunkCoordsRelZ = MathUtils.Mod(coords.Z, ChunkCount);
        return new Coords2(chunkCoordsRelX, chunkCoordsRelZ);
    }

    public static Coords2 GetRegionCoordsFromChunkCoordsAbs(Coords2 chunkCoordsAbs)
    {
        return new(MathUtils.DivFloor(chunkCoordsAbs.X, ChunkCount),
                   MathUtils.DivFloor(chunkCoordsAbs.Z, ChunkCount));
    }

    public (ReadOnlyCollection<Coords2> generatedChunksList, HashSet<Coords2> generatedChunksSet) GetGeneratedChunksAsCoordsRel()
    {
        HashSet<Coords2> generatedChunksSet = new(TotalChunkCount);
        for (int x = 0; x < ChunkCount; x++)
        {
            for (int z = 0; z < ChunkCount; z++)
            {
                Coords2 coordsChunk = new(x, z);
                if (HasChunkGenerated(coordsChunk))
                    generatedChunksSet.Add(coordsChunk);
            }
        }
        generatedChunksSet.TrimExcess();
        return (new ReadOnlyCollection<Coords2>(generatedChunksSet.ToList()), generatedChunksSet);
    }

    public bool HasChunkGenerated(Coords2 chunkCoordsRel)
    {
        (int sectorPos, int sectorLength) = GetChunkHeaderData(chunkCoordsRel);
        return HasChunkGenerated(sectorPos, sectorLength);
    }

    private static bool HasChunkGenerated(int sectorPos, int sectorLength)
    {
        if (sectorPos == 0 && sectorLength == 0)
            return false;
        return true;
    }

    private (int sectorPos, int sectorLength) GetChunkHeaderData(Coords2 chunkCoordsRel)
    {
        ChunkRangeRel.ThrowIfOutside(chunkCoordsRel);
        int seekPos = (chunkCoordsRel.X + chunkCoordsRel.Z * ChunkCount) * ChunkHeaderSize;
        Span<byte> chunkHeaderSegment = Read(seekPos, ChunkHeaderSize);
        int chunkPos = 0;
        for (int i = 0; i < 3; i++)
        {
            int buff = chunkHeaderSegment[i];
            buff <<= (3 - i - 1) * 8;
            chunkPos += buff;
        }
        int chunkLength = chunkHeaderSegment[3];
        return (chunkPos, chunkLength);
    }

    private Span<byte> Read(int pos, int count)
    {
        return new Span<byte>(_data, pos, count);
    }

    public NbtCompound GetChunkNbt(Coords2 chunkCoords, bool relative)
    {
        Coords2 chunkCoordsRel;
        if (relative)
        {
            ChunkRangeRel.ThrowIfOutside(chunkCoords);
            chunkCoordsRel = chunkCoords;
        }
        else
        {
            ChunkRangeAbs.ThrowIfOutside(chunkCoords);
            chunkCoordsRel = ConvertChunkCoordsAbsToRel(chunkCoords);
        }

        (int sectorPos, int sectorLength) = GetChunkHeaderData(chunkCoordsRel);
        if (!HasChunkGenerated(sectorPos, sectorLength))
        {
            string msg = $"Chunk {chunkCoordsRel} (relative) is not generated yet";
            throw new ChunkNotGeneratedException(msg);
        }

        int seekPos = sectorPos * SectorDataSize;
        int dataLength = sectorLength * SectorDataSize;

        using MemoryStream chunkSectorStream = new(_data, seekPos, dataLength, false);
        using BinaryReaderEndian reader = new(chunkSectorStream);
        int chunkNbtLength = reader.ReadInt(ByteOrder.BigEndian);
        chunkNbtLength -= 1;
        int compressionMethod = reader.ReadByte();
        int chunkNbtDataPos = (int)(seekPos + chunkSectorStream.Position);
        int chunkNbtDataLength = (int)(dataLength - chunkSectorStream.Position);

        using MemoryStream chunkNbtStream = new(_data, chunkNbtDataPos, chunkNbtDataLength, false);
        NbtCompound chunkNbt = (NbtCompound)NbtIO.ReadStream(chunkNbtStream);
        return chunkNbt;
    }

    public Chunk GetChunk(Coords2 chunkCoords, bool relative)
    {
        return new Chunk(GetChunkNbt(chunkCoords, relative));
    }

    public override string ToString()
    {
        return $"Region {RegionCoords} at \"{_path}\"";
    }
}
