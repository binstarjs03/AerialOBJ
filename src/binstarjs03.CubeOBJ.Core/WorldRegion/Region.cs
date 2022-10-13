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
    public static readonly int ChunkSHeaderSize = 4;

    private readonly string _path;
    private readonly Stream _stream;
    private readonly Coords2 _coords;
    private readonly CoordsRange2 _chunkRangeAbs;
    private bool _hasDisposed;

    public Region(string path, Coords2 coords)
    {
        _path = path;
        FileInfo fi = new(path);
        checkRegionData(fi);
        _stream = File.Open(
            path,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read);
        _coords = coords;
        _chunkRangeAbs = evaluateChunkRangeAbs(coords);

        static void checkRegionData(FileInfo fileInfo)
        {
            if (fileInfo.Length > ChunkHeaderTableSize)
                return;
            string msg = "Region data is too small";
            throw new InvalidDataException(msg);
        }

        static CoordsRange2 evaluateChunkRangeAbs(Coords2 coords)
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

    #region Dispose Pattern

    protected virtual void Dispose(bool disposing)
    {
        if (!_hasDisposed)
        {
            //if (disposing) {
            //    dispose managed objects
            //}
            // set large fields to null
            _stream.Dispose();
            _hasDisposed = true;
        }
    }

    ~Region()
    {
        Dispose(disposing: false);
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    #endregion

    public Coords2 Coords => _coords;

    public CoordsRange2 ChunkRangeAbs => _chunkRangeAbs;

    public static Coords2 ConvertChunkAbsToRel(Coords2 coords)
    {
        int relCx = MathUtils.Mod(coords.X, ChunkCount);
        int relCz = MathUtils.Mod(coords.Z, ChunkCount);
        return new Coords2(relCx, relCz);
    }

    public bool HasChunkGenerated(Coords2 coords, bool relative)
    {
        var (sectorPos, sectorLength) = GetChunkHeaderData(coords, relative);
        return HasChunkGenerated(sectorPos, sectorLength);
    }

    private static bool HasChunkGenerated(int sectorPos, int sectorLength)
    {
        if (sectorPos == 0 && sectorLength == 0)
            return false;
        return true;
    }

    private (int sectorPos, int sectorLength) GetChunkHeaderData(Coords2 coords, bool relative)
    {
        if (_hasDisposed)
            throw new ObjectDisposedException(nameof(_stream), "Region is already disposed");
        if (!relative)
        {
            ChunkRangeAbs.ThrowIfOutside(coords);
            coords = ConvertChunkAbsToRel(coords);
        }
        else
        {
            ChunkRangeRel.ThrowIfOutside(coords);
        }

        long seekPos = (coords.X + coords.Z * ChunkCount) * ChunkSHeaderSize;
        _stream.Seek(seekPos, SeekOrigin.Begin);
        byte[] chunkHeader = new byte[ChunkSHeaderSize];
        if (_stream.Read(chunkHeader) < ChunkSHeaderSize)
            throw new EndOfStreamException();

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
                if (HasChunkGenerated(coordsChunk, relative: true))
                    generatedChunks.Add(coordsChunk);
            }
        }
        return generatedChunks.ToArray();
    }

    public Chunk GetChunk(Coords2 coords, bool relative)
    {
        using (MemoryStream chunkSectorStream = GetChunkRawStream(coords, relative))
        using (IO.BinaryReaderEndian reader = new(chunkSectorStream, IO.ByteOrder.BigEndian))
        {
            int nbtChunkLength = reader.ReadInt();
            NbtCompression.Method compressionMethod = (NbtCompression.Method)reader.ReadByte();
            byte[] compressedNbtData = reader.ReadBytes(nbtChunkLength, endianMatter: false);

            NbtCompound nbtChunk = (NbtCompound)NbtBase.ReadStream(
                new MemoryStream(compressedNbtData),
                IO.ByteOrder.BigEndian,
                compressionMethod);
            if (!relative)
                coords = ConvertChunkAbsToRel(coords);
            return new Chunk(nbtChunk, coords);
        }
    }

    public static Chunk GetChunk(MemoryStream chunkStream, Coords2 coords, bool relative)
    {
        using (chunkStream)
        using (IO.BinaryReaderEndian reader = new(chunkStream, IO.ByteOrder.BigEndian))
        {
            int nbtChunkLength = reader.ReadInt();
            NbtCompression.Method compressionMethod = (NbtCompression.Method)reader.ReadByte();
            byte[] compressedNbtData = reader.ReadBytes(nbtChunkLength, endianMatter: false);

            NbtCompound nbtChunk = (NbtCompound)NbtBase.ReadStream(
                new MemoryStream(compressedNbtData),
                IO.ByteOrder.BigEndian,
                compressionMethod);
            if (!relative)
                coords = ConvertChunkAbsToRel(coords);
            return new Chunk(nbtChunk, coords);
        }
    }

    public MemoryStream GetChunkRawStream(Coords2 coords, bool relative)
    {
        if (_hasDisposed)
            throw new ObjectDisposedException(nameof(_stream), "Region is already disposed");
        var (sectorPos, sectorLength) = GetChunkHeaderData(coords, relative);
        if (!HasChunkGenerated(sectorPos, sectorLength))
        {
            string msg = $"Chunk is not generated yet";
            throw new ChunkNotGeneratedException(msg);
        }

        long seekPos = sectorPos * SectorDataSize;
        int dataLength = sectorLength * SectorDataSize;
        _stream.Seek(seekPos, SeekOrigin.Begin);
        byte[] chunkSectorData = new byte[dataLength];
        if (_stream.Read(chunkSectorData) < dataLength)
            throw new EndOfStreamException();

        return new MemoryStream(chunkSectorData);
    }

    public override string ToString()
    {
        string ioStatus = _stream.CanRead ? "Open" : "Closed";
        return $"Region {Coords} at \"{_path}\", stream status: {ioStatus}";
    }
}
