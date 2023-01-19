using binstarjs03.AerialOBJ.Core.Primitives;

using static binstarjs03.AerialOBJ.Core.MathUtils;

namespace binstarjs03.AerialOBJ.Core.MinecraftWorld;
public class MinecraftWorldMathUtils
{
    /// <summary>
    /// Returns the relative coordinate version of absolute chunk coordinate (relative to its region)
    /// </summary>
    public static PointZ<int> ConvertChunkCoordsAbsToRel(PointZ<int> coords)
    {
        int chunkCoordsRelX = Mod(coords.X, IRegion.ChunkCount);
        int chunkCoordsRelZ = Mod(coords.Z, IRegion.ChunkCount);
        return new PointZ<int>(chunkCoordsRelX, chunkCoordsRelZ);
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

    public static PointZ<int> ConvertBlockCoordsAbsToRelToChunk(PointZ<int> coords)
    {
        int blockCoordsAbsX = Mod(coords.X, IChunk.BlockCount);
        int blockCoordsAbsZ = Mod(coords.Z, IChunk.BlockCount);
        return new PointZ<int>(blockCoordsAbsX, blockCoordsAbsZ);
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
    public static PointZRange<int> CalculateChunkRangeAbsForRegion(PointZ<int> regionCoords)
    {
        int chunkRangeAbsMinX = regionCoords.X * IRegion.ChunkCount;
        int chunkRangeAbsMaxX = chunkRangeAbsMinX + IRegion.ChunkRange;
        Rangeof<int> chunkRangeAbsX = new(chunkRangeAbsMinX, chunkRangeAbsMaxX);

        int chunkRangeAbsMinZ = regionCoords.Z * IRegion.ChunkCount;
        int chunkRangeAbsMaxZ = chunkRangeAbsMinZ + IRegion.ChunkRange;
        Rangeof<int> chunkRangeAbsZ = new(chunkRangeAbsMinZ, chunkRangeAbsMaxZ);

        return new PointZRange<int>(chunkRangeAbsX, chunkRangeAbsZ);
    }

    /// <summary>
    /// Returns absolute chunk coords for given absolute 2D block coords
    /// </summary>
    public static PointZ<int> GetChunkCoordsAbsFromBlockCoordsAbs(PointZ<int> blockCoordsAbs)
    {
        return new PointZ<int>(DivFloor(blockCoordsAbs.X, IChunk.BlockCount),
                                DivFloor(blockCoordsAbs.Z, IChunk.BlockCount));
    }

    /// <summary>
    /// Returns absolute chunk coords for given absolute 3D block coords
    /// </summary>
    public static PointZ<int> GetChunkCoordsAbsFromBlockCoordsAbs(Point3<int> blockCoordsAbs)
    {
        return new PointZ<int>(DivFloor(blockCoordsAbs.X, IChunk.BlockCount),
                                DivFloor(blockCoordsAbs.Z, IChunk.BlockCount));
    }

    /// <summary>
    /// Returns region coords for given absolute chunk coords
    /// </summary>
    public static PointZ<int> GetRegionCoordsFromChunkCoordsAbs(PointZ<int> chunkCoordsAbs)
    {
        return new(DivFloor(chunkCoordsAbs.X, IRegion.ChunkCount),
                   DivFloor(chunkCoordsAbs.Z, IRegion.ChunkCount));
    }
}
