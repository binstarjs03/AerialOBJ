using System;
using System.IO;

using binstarjs03.AerialOBJ.Core.MinecraftWorld;
using binstarjs03.AerialOBJ.Core.Primitives;

using CoordsConversion = binstarjs03.AerialOBJ.Core.MathUtils.MinecraftCoordsConversion;

namespace binstarjs03.AerialOBJ.Core.MinecraftWorldRefactor;
public class Region
{
    public const int ChunkCount = 32;
    public const int TotalChunkCount = ChunkCount * ChunkCount;
    public const int ChunkRange = ChunkCount - 1;
    public static readonly Point2ZRange<int> ChunkRangeRel = new(Point2Z<int>.Zero, new Point2Z<int>(ChunkRange, ChunkRange));

    public const int SectorDataLength = 4096;
    public const int ChunkHeaderTableSize = SectorDataLength * 1;
    public const int ChunkHeaderSize = 4;

    private readonly string _sourcePath;
    private readonly byte[] _data;

    public Region(string path, Point2Z<int> regionCoords)
    {
        if (!File.Exists(path))
            throw new FileNotFoundException("Region file does not exist from given path");
        verifyDataLength(new FileInfo(path).Length);
        _sourcePath = path;
        _data = File.ReadAllBytes(path);
        Coords = regionCoords;
        ChunkRangeAbs = CoordsConversion.CalculateChunkRangeAbsForRegion(regionCoords);
        static void verifyDataLength(long dataLength)
        {
            if (dataLength == 0)
                throw new RegionNoDataException();
            if (dataLength < ChunkHeaderTableSize)
                throw new InvalidDataException("Region data is too small");
        }
    }

    public Point2Z<int> Coords { get; }
    public Point2ZRange<int> ChunkRangeAbs { get; }

    public IChunk GetChunk(Point2Z<int> chunkCoordsRel)
    {
        throw new NotImplementedException();
    }

    public override string ToString()
    {
        return $"Region {Coords}";
    }
}
