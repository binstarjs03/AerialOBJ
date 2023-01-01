using System;
using System.Collections.Generic;

using binstarjs03.AerialOBJ.Core.Nbt;
using binstarjs03.AerialOBJ.Core.Primitives;

using CoordsConversion = binstarjs03.AerialOBJ.Core.MathUtils.MinecraftCoordsConversion;

namespace binstarjs03.AerialOBJ.Core.MinecraftWorldRefactor;
public class Chunk2860 : IChunk
{
    private readonly Dictionary<int, Section> _sections;
    private readonly int[] _sectionsY;

    public Chunk2860(NbtCompound chunkNbt)
    {
        CoordsAbs = getChunkCoordsAbs(chunkNbt);
        CoordsRel = CoordsConversion.ConvertChunkCoordsAbsToRel(CoordsAbs);
        BlockRangeAbs = CoordsConversion.CalculateBlockRangeAbsForChunk(CoordsAbs);

        static Point2Z<int> getChunkCoordsAbs(NbtCompound chunkNbt)
        {
            int chunkCoordsAbsX = chunkNbt.Get<NbtInt>("xPos").Value;
            int chunkCoordsAbsZ = chunkNbt.Get<NbtInt>("zPos").Value;
            return new Point2Z<int>(chunkCoordsAbsX, chunkCoordsAbsZ);
        }
    }

    public Point2Z<int> CoordsAbs { get; }
    public Point2Z<int> CoordsRel { get; }
    public Point3Range<int> BlockRangeAbs { get; }
    public int DataVersion => 2860;
    public string ReleaseVersion => "1.18";

    public void GetHighestBlock(ChunkHighestBlockBuffer highestBlockBuffer)
    {
        for (int z = 0; z < IChunk.BlockCount; z++)
            for (int x = 0; x < IChunk.BlockCount; x++)
            {
                bool foundHighestBlock = false;
                for (int index = _sectionsY.Length - 1; index >= 0; index--)
                {
                    if (foundHighestBlock)
                        break;
                    int sectionY = _sectionsY[index];
                    Section section = _sections[sectionY];
                    int heightAtCurrentSection = section.YPos * IChunk.BlockCount;

                    for (int y = IChunk.BlockRange; y >= 0; y--)
                    {
                        if (foundHighestBlock)
                            break;
                        // height in here means current block global Y position
                        int height = heightAtCurrentSection + y;

                        Point3<int> blockCoordsRel = new(x, y, z);
                        Block block = section.GetBlock(blockCoordsRel);
                        if (block.IsAir)
                            continue;
                        highestBlockBuffer.Names[x, z] = block.Name;
                        highestBlockBuffer.Heights[x, z] = height;
                        foundHighestBlock = true;
                    }
                }
            }
    }

    private class Section
    {
        private readonly int[,,]? _blockPaletteIndexTable;
        private readonly Block[]? _blockPalette;

        public required int YPos { get; init; }

        public Block GetBlock(Point3<int> blockCoordsRel)
        {
            throw new NotImplementedException();
        }
    }
}
