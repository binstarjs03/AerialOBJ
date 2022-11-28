using binstarjs03.AerialOBJ.Core.Primitives;

namespace binstarjs03.AerialOBJ.Core.MinecraftWorld;
public static class CoordsUtils
{
    public static Point2Z<int> ConvertChunkCoordsAbsToRel(Point2Z<int> coords)
    {
        int chunkCoordsRelX = MathUtils.Mod(coords.X, Region.ChunkCount);
        int chunkCoordsRelZ = MathUtils.Mod(coords.Z, Region.ChunkCount);
        return new Point2Z<int>(chunkCoordsRelX, chunkCoordsRelZ);
    }

    public static Point2Z<int> GetChunkRegionCoords(Point2Z<int> chunkCoordsAbs)
    {
        return new(MathUtils.DivFloor(chunkCoordsAbs.X, Region.ChunkCount),
                   MathUtils.DivFloor(chunkCoordsAbs.Z, Region.ChunkCount));
    }

    public static Point3Range<int> CalculateChunkBlockRangeAbs(Point2Z<int> chunkCoordsAbs)
    {
        int blockRangeAbsMinX = chunkCoordsAbs.X * Section.BlockCount;
        int blockRangeAbsMinY = Chunk.SectionRange.Min * Section.BlockCount;
        int blockRangeAbsMinZ = chunkCoordsAbs.Z * Section.BlockCount;
        Point3<int> blockRangeAbsMin = new(blockRangeAbsMinX, blockRangeAbsMinY, blockRangeAbsMinZ);

        int blockRangeAbsMaxX = blockRangeAbsMinX + Section.BlockRange;
        int blockRangeAbsMaxY = Chunk.SectionRange.Max * Section.BlockCount + Section.BlockRange;
        int blockRangeAbsMaxZ = blockRangeAbsMinZ + Section.BlockRange;
        Point3<int> blockRangeAbsMax = new(blockRangeAbsMaxX, blockRangeAbsMaxY, blockRangeAbsMaxZ);

        return new Point3Range<int>(blockRangeAbsMin, blockRangeAbsMax);
    }

    /// <summary>
    /// Convert block absolute coordinate to local block coordinate of chunk
    /// </summary>
    /// <param name="coords">Block absolute coordinate</param>
    /// <returns>Block coordinate relative to chunk</returns>
    public static Point3<int> ConvertChunkBlockCoordsAbsToRel(Point3<int> coords)
    {
        int blockCoordsAbsX = MathUtils.Mod(coords.X, Section.BlockCount);
        int blockCoordsAbsY = coords.Y;
        int blockCoordsAbsZ = MathUtils.Mod(coords.Z, Section.BlockCount);
        return new Point3<int>(blockCoordsAbsX, blockCoordsAbsY, blockCoordsAbsZ);
    }

    public static Point3Range<int> CalculateSectionBlockRangeAbs(Point3<int> coordsAbs)
    {
        int minAbsBx = coordsAbs.X * Section.BlockCount;
        int minAbsBy = coordsAbs.Y * Section.BlockCount;
        int minAbsBz = coordsAbs.Z * Section.BlockCount;
        Point3<int> minAbsB = new(minAbsBx, minAbsBy, minAbsBz);

        int maxAbsBx = minAbsBx + Section.BlockRange;
        int maxAbsBy = minAbsBy + Section.BlockRange;
        int maxAbsBz = minAbsBz + Section.BlockRange;
        Point3<int> maxAbsB = new(maxAbsBx, maxAbsBy, maxAbsBz);

        return new Point3Range<int>(minAbsB, maxAbsB);
    }

    public static Point2Z<int> GetChunkCoordsAbsFromBlockCoordsAbs(Point2Z<int> blockCoordsAbs)
    {
        return new Point2Z<int>(MathUtils.DivFloor(blockCoordsAbs.X, Section.BlockCount),
                           MathUtils.DivFloor(blockCoordsAbs.Z, Section.BlockCount));
    }

    public static Point3<int> ConvertSectionBlockCoordsAbsToRel(Point3<int> coords)
    {
        int blockCoordsAbsX = MathUtils.Mod(coords.X, Section.BlockCount);
        int blockCoordsAbsY = MathUtils.Mod(coords.Y, Section.BlockCount);
        int blockCoordsAbsZ = MathUtils.Mod(coords.Z, Section.BlockCount);
        return new Point3<int>(blockCoordsAbsX, blockCoordsAbsY, blockCoordsAbsZ);
    }
}
