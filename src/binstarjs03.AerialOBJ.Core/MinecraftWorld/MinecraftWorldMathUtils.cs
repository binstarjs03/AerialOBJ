using binstarjs03.AerialOBJ.Core.Primitives;

using static binstarjs03.AerialOBJ.Core.MathUtils;

namespace binstarjs03.AerialOBJ.Core.MinecraftWorld;
public class MinecraftWorldMathUtils
{
    /// <summary>
    /// Returns the relative coordinate version of absolute chunk coordinate (relative to its region)
    /// </summary>
    public static Point2Z<int> ConvertChunkCoordsAbsToRel(Point2Z<int> coords)
    {
        int chunkCoordsRelX = Mod(coords.X, Region.ChunkCount);
        int chunkCoordsRelZ = Mod(coords.Z, Region.ChunkCount);
        return new Point2Z<int>(chunkCoordsRelX, chunkCoordsRelZ);
    }

    /// <summary>
    /// Convert block absolute coordinate to local block coordinate of chunk
    /// </summary>
    /// <param name="coords">Block absolute coordinate</param>
    /// <returns>Block coordinate relative to chunk</returns>
    public static Point3<int> ConvertBlockCoordsAbsToRelToChunk(Point3<int> coords)
    {
        int blockCoordsAbsX = Mod(coords.X, IChunk.BlockCount);
        int blockCoordsAbsY = coords.Y;
        int blockCoordsAbsZ = Mod(coords.Z, IChunk.BlockCount);
        return new Point3<int>(blockCoordsAbsX, blockCoordsAbsY, blockCoordsAbsZ);
    }

    public static Point2Z<int> ConvertBlockCoordsAbsToRelToChunk(Point2Z<int> coords)
    {
        int blockCoordsAbsX = Mod(coords.X, IChunk.BlockCount);
        int blockCoordsAbsZ = Mod(coords.Z, IChunk.BlockCount);
        return new Point2Z<int>(blockCoordsAbsX, blockCoordsAbsZ);
    }

    public static Point3<int> ConvertBlockCoordsAbsToRelToSection(Point3<int> coords)
    {
        int blockCoordsAbsX = Mod(coords.X, IChunk.BlockCount);
        int blockCoordsAbsY = Mod(coords.Y, IChunk.BlockCount);
        int blockCoordsAbsZ = Mod(coords.Z, IChunk.BlockCount);
        return new Point3<int>(blockCoordsAbsX, blockCoordsAbsY, blockCoordsAbsZ);
    }

    public static Point3<int> ConvertBlockCoordsRelToSectionToAbs(Point3<int> blockCoordsRel, Point3<int> sectionCoordsAbs)
    {
        int blockCoordsAbsX = sectionCoordsAbs.X * IChunk.BlockCount + blockCoordsRel.X;
        int blockCoordsAbsY = sectionCoordsAbs.Y * IChunk.BlockCount + blockCoordsRel.Y;
        int blockCoordsAbsZ = sectionCoordsAbs.Z * IChunk.BlockCount + blockCoordsRel.Z;
        return new Point3<int>(blockCoordsAbsX, blockCoordsAbsY, blockCoordsAbsZ);
    }

    /// <summary>
    /// Calculates range of absolute chunk coords for given region coords
    /// </summary>
    public static Point2ZRange<int> CalculateChunkRangeAbsForRegion(Point2Z<int> regionCoords)
    {
        int chunkRangeAbsMinX = regionCoords.X * Region.ChunkCount;
        int chunkRangeAbsMaxX = chunkRangeAbsMinX + Region.ChunkRange;
        Rangeof<int> chunkRangeAbsX = new(chunkRangeAbsMinX, chunkRangeAbsMaxX);

        int chunkRangeAbsMinZ = regionCoords.Z * Region.ChunkCount;
        int chunkRangeAbsMaxZ = chunkRangeAbsMinZ + Region.ChunkRange;
        Rangeof<int> chunkRangeAbsZ = new(chunkRangeAbsMinZ, chunkRangeAbsMaxZ);

        return new Point2ZRange<int>(chunkRangeAbsX, chunkRangeAbsZ);
    }

    /// <summary>
    /// Returns absolute chunk coords for given absolute 2D block coords
    /// </summary>
    public static Point2Z<int> GetChunkCoordsAbsFromBlockCoordsAbs(Point2Z<int> blockCoordsAbs)
    {
        return new Point2Z<int>(DivFloor(blockCoordsAbs.X, IChunk.BlockCount),
                                DivFloor(blockCoordsAbs.Z, IChunk.BlockCount));
    }

    /// <summary>
    /// Returns absolute chunk coords for given absolute 3D block coords
    /// </summary>
    public static Point2Z<int> GetChunkCoordsAbsFromBlockCoordsAbs(Point3<int> blockCoordsAbs)
    {
        return new Point2Z<int>(DivFloor(blockCoordsAbs.X, IChunk.BlockCount),
                                DivFloor(blockCoordsAbs.Z, IChunk.BlockCount));
    }

    /// <summary>
    /// Returns region coords for given absolute chunk coords
    /// </summary>
    public static Point2Z<int> GetRegionCoordsFromChunkCoordsAbs(Point2Z<int> chunkCoordsAbs)
    {
        return new(DivFloor(chunkCoordsAbs.X, Region.ChunkCount),
                   DivFloor(chunkCoordsAbs.Z, Region.ChunkCount));
    }
}
