using System;
using System.Buffers.Binary;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using binstarjs03.AerialOBJ.Core.CoordinateSystem;
using binstarjs03.AerialOBJ.Core.Nbt;
using binstarjs03.AerialOBJ.Core.Nbt.IO;

namespace binstarjs03.AerialOBJ.Core.WorldRegion;

public class Region : IDisposable
{
    public static readonly int ChunkCount = 32;
    public static readonly int TotalChunkCount = (int)Math.Pow(ChunkCount, 2);
    public static readonly int ChunkRange = ChunkCount - 1;
    public static readonly CoordsRange2 ChunkRangeRel = new(
        min: Coords2.Zero,
        max: new Coords2(ChunkRange, ChunkRange)
    );

    public static readonly int SectorDataSize = 4096;
    public static readonly int ChunkHeaderTableSize = SectorDataSize * 1;
    public static readonly int ChunkHeaderSize = 4;

    private readonly string _path;
    private byte[]? _data;

    private readonly Coords2 _coords;
    private readonly CoordsRange2 _chunkRangeAbs;

    private bool _hasDisposed;

    public Region(string path, Coords2 coords)
    {
        FileInfo fi = new(path);
        checkRegionData(fi);
        _path = path;
        _data = File.ReadAllBytes(path);
        _coords = coords;
        _chunkRangeAbs = calculateChunkRangeAbs(coords);

        static void checkRegionData(FileInfo fileInfo)
        {
            if (fileInfo.Length > ChunkHeaderTableSize)
                return;
            string msg = "Region data is too small";
            throw new InvalidDataException(msg);
        }

        static CoordsRange2 calculateChunkRangeAbs(Coords2 coords)
        {
            int minAbsCx = coords.X * ChunkCount;
            int minAbsCz = coords.Z * ChunkCount;
            Coords2 minAbsC = new(minAbsCx, minAbsCz);

            int maxAbsCx = minAbsCx + ChunkRange;
            int maxAbsCz = minAbsCz + ChunkRange;
            Coords2 maxAbsC = new(maxAbsCx, maxAbsCz);

            return new CoordsRange2(minAbsC, maxAbsC);
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
        {
            throw new RegionUnrecognizedFileException("Cannot automatically determine region position");
        }
    }

    #region Dispose Pattern

    protected virtual void Dispose(bool disposing)
    {
        if (!_hasDisposed)
        {
            _data = null;
            _hasDisposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    #endregion

    public Coords2 Coords => _coords;
    public CoordsRange2 ChunkRangeAbs => _chunkRangeAbs;

    /// <exception cref="ObjectDisposedException"></exception>
    /// <exception cref="EndOfStreamException"></exception>
    private byte[] Read(long pos, int count)
    {
        if (_hasDisposed)
            throw new ObjectDisposedException(nameof(Region), "Region is already disposed");
        byte[] buff = new byte[count];
        int readCount;

        MemoryStream stream = new(_data!);
        stream.Seek(pos, SeekOrigin.Begin);
        readCount = stream.Read(buff);
        stream.Close();

        if (readCount < count)
            throw new EndOfStreamException();
        return buff;
    }

    private byte Read(long pos)
    {
        return Read(pos, 1)[0];
    }

    public static Coords2 ConvertChunkCoordsAbsToRel(Coords2 coords)
    {
        int relCx = MathUtils.Mod(coords.X, ChunkCount);
        int relCz = MathUtils.Mod(coords.Z, ChunkCount);
        return new Coords2(relCx, relCz);
    }

    public bool HasChunkGenerated(Coords2 chunkCoordsRel)
    {
        var (sectorPos, sectorLength) = GetChunkHeaderData(chunkCoordsRel);
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

        long seekPos = (chunkCoordsRel.X + chunkCoordsRel.Z * ChunkCount) * ChunkHeaderSize;
        byte[] chunkHeader = Read(seekPos, ChunkHeaderSize);

        int chunkPos = BinaryPrimitives.ReadInt32BigEndian(new byte[1].Concat(chunkHeader[0..3]).ToArray());
        int chunkLength = chunkHeader[3];
        return (chunkPos, chunkLength);
    }

    public Coords2[] GetGeneratedChunksAsCoords()
    {
        List<Coords2> generatedChunks = new();
        for (int x = 0; x < ChunkCount; x++)
        {
            for (int z = 0; z < ChunkCount; z++)
            {
                Coords2 coordsChunk = new(x, z);
                if (HasChunkGenerated(coordsChunk))
                    generatedChunks.Add(coordsChunk);
            }
        }
        return generatedChunks.ToArray();
    }

    public Chunk GetChunk(Coords2 chunkCoords, bool relative)
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

        var (sectorPos, sectorLength) = GetChunkHeaderData(chunkCoordsRel);
        if (!HasChunkGenerated(sectorPos, sectorLength))
        {
            string msg = $"Chunk is not generated yet";
            throw new ChunkNotGeneratedException(msg);
        }

        long seekPos = sectorPos * SectorDataSize;
        int dataLength = sectorLength * SectorDataSize;
        byte[] chunkSectorData = Read(seekPos, dataLength);

        using (MemoryStream chunkSectorStream = new(chunkSectorData))
        using (IO.BinaryReaderEndian reader = new(chunkSectorStream, IO.ByteOrder.BigEndian))
        {
            int nbtChunkLength = reader.ReadInt();
            NbtCompression.Method compressionMethod = (NbtCompression.Method)reader.ReadByte();
            byte[] compressedNbtData = reader.ReadBytes(nbtChunkLength, endianMatter: false);

            NbtCompound nbtChunk = (NbtCompound)NbtBase.ReadStream(
                new MemoryStream(compressedNbtData),
                IO.ByteOrder.BigEndian,
                compressionMethod);
            return new Chunk(nbtChunk);
        }
    }

    public override string ToString()
    {
        string disposeStatus = _hasDisposed ? "Disposed" : "Ready";
        return $"Region {Coords} at \"{_path}\", status: {disposeStatus}";
    }
}
