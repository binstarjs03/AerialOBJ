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
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

using binstarjs03.AerialOBJ.Core.Primitives;
using binstarjs03.AerialOBJ.Core.Nbt;

namespace binstarjs03.AerialOBJ.Core.MinecraftWorld;

public class Region
{
    public const int BlockCount = Section.BlockCount * ChunkCount;
    public const int ChunkCount = 32;
    public const int TotalChunkCount = ChunkCount * ChunkCount;
    public const int ChunkRange = ChunkCount - 1;
    public static readonly Point2ZRange<int> ChunkRangeRel = new(
        min: Point2Z<int>.Zero,
        max: new Point2Z<int>(ChunkRange, ChunkRange)
    );

    public const int SectorDataLength = 4096;
    public const int ChunkHeaderTableSize = SectorDataLength * 1;
    public const int ChunkHeaderSize = 4;

    private readonly string _path;
    private readonly byte[] _data;
    private readonly Point2Z<int> _regionCoords;
    private readonly Point2ZRange<int> _chunkRangeAbs;

    public Point2Z<int> RegionCoords => _regionCoords;
    public Point2ZRange<int> ChunkRangeAbs => _chunkRangeAbs;

    public Region(string path, Point2Z<int> regionCoords)
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
        static Point2ZRange<int> calculateChunkRangeAbs(Point2Z<int> regionCoords)
        {
            int chunkRangeAbsMinX = regionCoords.X * ChunkCount;
            int chunkRangeAbsMaxX = chunkRangeAbsMinX + ChunkRange;
            Rangeof<int> chunkRangeAbsX = new(chunkRangeAbsMinX, chunkRangeAbsMaxX);

            int chunkRangeAbsMinZ = regionCoords.Z * ChunkCount;
            int chunkRangeAbsMaxZ = chunkRangeAbsMinZ + ChunkRange;
            Rangeof<int> chunkRangeAbsZ = new(chunkRangeAbsMinZ, chunkRangeAbsMaxZ);

            return new Point2ZRange<int>(chunkRangeAbsX, chunkRangeAbsZ);
        }
    }

    public static Region Open(string path, Point2Z<int> coords)
        => new(path, coords);

    /// <exception cref="RegionUnrecognizedFileException"></exception>
    public static Region Open(string path)
    {
        FileInfo fi = new(path);
        if (IsValidFilename(fi.Name, out Point2Z<int>? regionCoords))
            return new Region(path, regionCoords!.Value);
        throw new RegionUnrecognizedFileException("Cannot automatically determine region position");
    }

    public static bool IsValidFilename(string regionFilename, out Point2Z<int>? regionCoords)
    {
        regionCoords = null;
        string[] split = regionFilename.Split('.');
        if (split.Length < 4)
            return false;

        bool correctPrefix = split[0] == "r";
        bool correctFileType = split[3] == "mca";
        bool validX = int.TryParse(split[1], out int x);
        bool validZ = int.TryParse(split[2], out int z);
        bool validCoordinate = validX && validZ;

        if (correctPrefix && correctFileType && validCoordinate)
        {
            regionCoords = new Point2Z<int>(x, z);
            return true;
        }
        else
            return false;
    }

    public static Point2Z<int> ConvertChunkCoordsAbsToRel(Point2Z<int> coords)
    {
        int chunkCoordsRelX = MathUtils.Mod(coords.X, ChunkCount);
        int chunkCoordsRelZ = MathUtils.Mod(coords.Z, ChunkCount);
        return new Point2Z<int>(chunkCoordsRelX, chunkCoordsRelZ);
    }

    public static Point2Z<int> GetRegionCoordsFromChunkCoordsAbs(Point2Z<int> chunkCoordsAbs)
    {
        return new(MathUtils.DivFloor(chunkCoordsAbs.X, ChunkCount),
                   MathUtils.DivFloor(chunkCoordsAbs.Z, ChunkCount));
    }

    public (ReadOnlyCollection<Point2Z<int>> generatedChunksList, HashSet<Point2Z<int>> generatedChunksSet) GetGeneratedChunksAsCoordsRel()
    {
        HashSet<Point2Z<int>> generatedChunksSet = new(TotalChunkCount);
        for (int x = 0; x < ChunkCount; x++)
        {
            for (int z = 0; z < ChunkCount; z++)
            {
                Point2Z<int> coordsChunk = new(x, z);
                if (HasChunkGenerated(coordsChunk))
                    generatedChunksSet.Add(coordsChunk);
            }
        }
        generatedChunksSet.TrimExcess();
        return (new ReadOnlyCollection<Point2Z<int>>(generatedChunksSet.ToList()), generatedChunksSet);
    }

    public HashSet<Point2Z<int>> GetGeneratedChunksAsCoordsRelSet()
    {
        HashSet<Point2Z<int>> generatedChunksSet = new(TotalChunkCount);
        for (int x = 0; x < ChunkCount; x++)
            for (int z = 0; z < ChunkCount; z++)
            {
                Point2Z<int> coordsChunk = new(x, z);
                if (HasChunkGenerated(coordsChunk))
                    generatedChunksSet.Add(coordsChunk);
            }
        generatedChunksSet.TrimExcess();
        return generatedChunksSet;
    }

    public bool HasChunkGenerated(Point2Z<int> chunkCoordsRel)
    {
        return HasChunkGenerated(GetChunkSectorTableEntryData(chunkCoordsRel));
    }

    private static bool HasChunkGenerated(ChunkSectorTableEntryData chunkSectorTableEntryData)
    {
        if (chunkSectorTableEntryData.SectorPos == 0 && chunkSectorTableEntryData.SectorSize == 0)
            return false;
        return true;
    }

    private ChunkSectorTableEntryData GetChunkSectorTableEntryData(Point2Z<int> chunkCoordsRel)
    {
        ChunkRangeRel.ThrowIfOutside(chunkCoordsRel);
        int seekPos = (chunkCoordsRel.X + chunkCoordsRel.Z * ChunkCount) * ChunkHeaderSize;
        Span<byte> binaryChunkSectorTableEntryData = Read(seekPos, ChunkHeaderSize);
        int sectorPos = 0;
        for (int i = 0; i < 3; i++)
        {
            int buff = binaryChunkSectorTableEntryData[i];
            buff <<= (3 - i - 1) * 8;
            sectorPos += buff;
        }
        int sectorSize = binaryChunkSectorTableEntryData[3];
        return new ChunkSectorTableEntryData(sectorPos, sectorSize);
    }

    private Span<byte> Read(int pos, int count)
    {
        return new Span<byte>(_data, pos, count);
    }

    public NbtCompound GetChunkNbt(Point2Z<int> chunkCoords, bool relative)
    {
        Point2Z<int> chunkCoordsRel;
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

        ChunkSectorTableEntryData chunkSectorTableEntryData = GetChunkSectorTableEntryData(chunkCoordsRel);
        if (!HasChunkGenerated(chunkSectorTableEntryData))
        {
            string msg = $"Chunk {chunkCoordsRel} (relative) is not generated yet";
            throw new ChunkNotGeneratedException(msg);
        }

        int seekPos = chunkSectorTableEntryData.SectorPos * SectorDataLength;
        int chunkNbtLength = BinaryPrimitives.ReadInt32BigEndian(Read(seekPos, 4)) - 1;
        int chunkNbtDataStart = seekPos + 5;

        using MemoryStream chunkNbtStream = new(_data, chunkNbtDataStart, chunkNbtLength, false);
        NbtCompound chunkNbt = (NbtIO.ReadStream(chunkNbtStream) as NbtCompound)!;
        return chunkNbt;
    }

    public Chunk GetChunk(Point2Z<int> chunkCoords, bool relative)
    {
        return new Chunk(GetChunkNbt(chunkCoords, relative));
    }

    public override string ToString()
    {
        return $"Region {RegionCoords} at \"{_path}\"";
    }
}
