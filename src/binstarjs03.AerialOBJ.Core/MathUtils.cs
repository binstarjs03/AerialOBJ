using System;

using binstarjs03.AerialOBJ.Core.MinecraftWorld;
using binstarjs03.AerialOBJ.Core.Primitives;

namespace binstarjs03.AerialOBJ.Core;

public static class MathUtils
{
    public static int Mod(int left, int right)
    {
        int result = left % right;
        if (right >= 0 && result < 0 || right < 0 && result >= 0)
            result += right;
        return result;
    }

    public static int DivFloor(double num, double divisor)
    {
        return (int)Math.Floor(num / divisor);
    }

    public static int DivFloor(int num, int divisor)
    {
        return (int)MathF.Floor((float)num / divisor);
    }

    public static int Floor(this double num)
    {
        return (int)Math.Floor(num);
    }

    public static float Round(this float num)
    {
        return (float)Math.Round(num, 2);
    }

    public static class PointSpaceConversion
    {
        public static Point2<float> ConvertWorldPosToScreenPos(Point2Z<float> worldPos, Point2Z<float> cameraPos, float unitMultiplier, Size<float> screenSize)
        {
            float screenX = ConvertWorldPosToScreenPos(worldPos.X, cameraPos.X, unitMultiplier, screenSize.Width);
            float screenY = ConvertWorldPosToScreenPos(worldPos.Z, cameraPos.Z, unitMultiplier, screenSize.Height);
            return new Point2<float>(screenX, screenY);
        }

        public static Point2Z<float> ConvertScreenPosToWorldPos(Point2<float> screenPos, Point2Z<float> cameraPos, float unitMultiplier, Size<float> screenSize)
        {
            float worldX = ConvertScreenPosToWorldPos(screenPos.X, cameraPos.X, unitMultiplier, screenSize.Width);
            float worldZ = ConvertScreenPosToWorldPos(screenPos.Y, cameraPos.Z, unitMultiplier, screenSize.Height);
            return new Point2Z<float>(worldX, worldZ);
        }

        public static float ConvertWorldPosToScreenPos(float worldPos, float cameraPos, float unitMultiplier, float screenSize)
        {
            return -(cameraPos * unitMultiplier) + (screenSize / 2) + (worldPos * unitMultiplier);
        }

        public static float ConvertScreenPosToWorldPos(float screenPos, float cameraPos, float unitMultiplier, float screenSize)
        {
            return -(-(cameraPos * unitMultiplier) + (screenSize / 2) - screenPos) / unitMultiplier;
        }
    }

    public static class MinecraftCoordsConversion
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
            int blockCoordsAbsX = Mod(coords.X, Section.BlockCount);
            int blockCoordsAbsY = coords.Y;
            int blockCoordsAbsZ = Mod(coords.Z, Section.BlockCount);
            return new Point3<int>(blockCoordsAbsX, blockCoordsAbsY, blockCoordsAbsZ);
        }

        public static Point2Z<int> ConvertBlockCoordsAbsToRelToChunk(Point2Z<int> coords)
        {
            int blockCoordsAbsX = Mod(coords.X, Section.BlockCount);
            int blockCoordsAbsZ = Mod(coords.Z, Section.BlockCount);
            return new Point2Z<int>(blockCoordsAbsX, blockCoordsAbsZ);
        }

        public static Point3<int> ConvertBlockCoordsAbsToRelToSection(Point3<int> coords)
        {
            int blockCoordsAbsX = Mod(coords.X, Section.BlockCount);
            int blockCoordsAbsY = Mod(coords.Y, Section.BlockCount);
            int blockCoordsAbsZ = Mod(coords.Z, Section.BlockCount);
            return new Point3<int>(blockCoordsAbsX, blockCoordsAbsY, blockCoordsAbsZ);
        }

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

        public static Point3Range<int> CalculateBlockRangeAbsForSection(Point3<int> coordsAbs)
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

        public static Point3Range<int> CalculateBlockRangeAbsForChunk(Point2Z<int> chunkCoordsAbs)
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

        public static Point2Z<int> GetChunkCoordsAbsFromBlockCoordsAbs(Point2Z<int> blockCoordsAbs)
        {
            return new Point2Z<int>(DivFloor(blockCoordsAbs.X, Section.BlockCount),
                               DivFloor(blockCoordsAbs.Z, Section.BlockCount));
        }

        /// <summary>
        /// Returns region coords for specified chunk coords
        /// </summary>
        public static Point2Z<int> GetRegionCoordsFromChunkCoordsAbs(Point2Z<int> chunkCoordsAbs)
        {
            return new(DivFloor(chunkCoordsAbs.X, Region.ChunkCount),
                       DivFloor(chunkCoordsAbs.Z, Region.ChunkCount));
        }
    }
}
