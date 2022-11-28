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
using System.Linq;

using binstarjs03.AerialOBJ.Core.Primitives;
using binstarjs03.AerialOBJ.Core.Nbt;

namespace binstarjs03.AerialOBJ.Core.MinecraftWorld;

public class Section
{
    public const int BlockCount = 16;
    public const int TotalBlockCount = BlockCount * BlockCount * BlockCount;
    public const int BlockRange = BlockCount - 1;
    public static readonly Point3Range<int> BlockRangeRel = new(
        min: Point3<int>.Zero,
        max: new Point3<int>(BlockRange, BlockRange, BlockRange)
    );

    private readonly int _sectionYPos;
    private readonly Point3<int> _sectionCoordsAbs;
    private readonly Point3Range<int> _blockRangeAbs;

    private readonly int[,,]? _blockPaletteIndexTable;
    private readonly Block[]? _blockPalette;

    public Point3<int> SectionCoordsAbs => _sectionCoordsAbs;
    public Point3Range<int> BlockRangeAbs => _blockRangeAbs;

    public Section(Point2Z<int> chunkCoordsAbs, NbtCompound sectionNbt)
    {
        _sectionYPos = sectionNbt.Get<NbtByte>("Y").Value;
        _sectionCoordsAbs = new Point3<int>(chunkCoordsAbs.X, _sectionYPos, chunkCoordsAbs.Z);
        _blockRangeAbs = CoordsUtils.CalculateSectionBlockRangeAbs(_sectionCoordsAbs);

        sectionNbt.TryGet("block_states", out NbtCompound? blockStatesNbt);
        if (blockStatesNbt is null)
            return;
        blockStatesNbt.TryGet("palette", out NbtList<NbtCompound>? paletteBlockNbt);
        if (paletteBlockNbt is null)
            return;

        _blockPalette = new Block[paletteBlockNbt.Count];
        for (int i = 0; i < paletteBlockNbt.Count; i++)
        {
            NbtCompound blockPropertyNbt = paletteBlockNbt[i];
            // coordinate in block template of block table doesn't matter
            // so we set it to zero
            _blockPalette[i] = new Block(Point3<int>.Zero, blockPropertyNbt);
        }

        blockStatesNbt.TryGet("data", out NbtLongArray? dataNbt);
        if (dataNbt is null)
            return;
        _blockPaletteIndexTable = ReadNbtLongData(dataNbt, paletteBlockNbt.Count);
    }

    // TODO move this method out to CoordsUtils
    public (Point3<int> blockCoordsRel, Point3<int> blockCoordsAbs) CalculateBlockCoords(Point3<int> blockCoords, bool relative, bool skipTest = false)
    {
        Point3<int> blockCoordsRel;
        Point3<int> blockCoordsAbs;
        if (relative)
        {
            if (!skipTest)
                BlockRangeRel.ThrowIfOutside(blockCoords);
            blockCoordsRel = blockCoords;
            blockCoordsAbs = new Point3<int>(_sectionCoordsAbs.X * BlockCount + blockCoords.X,
                                         _sectionCoordsAbs.Y * BlockCount + blockCoords.Y,
                                         _sectionCoordsAbs.Z * BlockCount + blockCoords.Z);
        }
        else
        {
            if (!skipTest)
                BlockRangeAbs.ThrowIfOutside(blockCoords);
            blockCoordsRel = CoordsUtils.ConvertChunkBlockCoordsAbsToRel(blockCoords);
            blockCoordsAbs = blockCoords;
        }
        return (blockCoordsRel, blockCoordsAbs);
    }

    // get what block the block table returned from given input.
    // This method does generate heap so avoid calling this method at tight-loops
    public Block GetBlock(Point3<int> coords, bool relative)
    {
        (Point3<int> coordsRel, Point3<int> coordsAbs) = CalculateBlockCoords(coords, relative);
        if (_blockPaletteIndexTable is null)
        {
            Block block = Block.Air;
            block.BlockCoordsAbs = coordsAbs;
            return block;
        }
        else
        {
            int blockTableIndex = _blockPaletteIndexTable[coordsRel.X, coordsRel.Y, coordsRel.Z];
            Block blockPalette = _blockPalette![blockTableIndex];
            Block block = new(blockPalette.BlockCoordsAbs, blockPalette.Name);
            return block;
        }
    }

    // peek what the block table returned from given input.
    // does not generate heap since it just referencing palette block
    public Block GetBlockPalette(Point3<int> blockCoordsRel)
    {
        if (_blockPaletteIndexTable is null)
            return Block.Air;
        else
        {
            int blockTableIndex = _blockPaletteIndexTable[blockCoordsRel.X, blockCoordsRel.Y, blockCoordsRel.Z];
            return _blockPalette![blockTableIndex];
        }
    }

    // set all input block properties to block table block properties, if inequal.
    // does not generate heap, return true if setting is succeed
    public bool SetBlock(Block block, Point3<int> coordsRel, string[]? exclusions = null)
    {
        if (_blockPaletteIndexTable is null) // set to air block
            return false;
        else
        {
            int blockTableIndex = _blockPaletteIndexTable[coordsRel.X, coordsRel.Y, coordsRel.Z];
            Block blockTemplate = _blockPalette![blockTableIndex];
            if (blockTemplate.Name == Block.Air.Name)
                return false;
            if (exclusions is not null && exclusions.Contains(blockTemplate.Name))
                return false;
            block.Name = blockTemplate.Name;
            block.BlockCoordsAbs = new Point3<int>(SectionCoordsAbs.X * BlockCount + coordsRel.X,
                                          SectionCoordsAbs.Y * BlockCount + coordsRel.Y,
                                          SectionCoordsAbs.Z * BlockCount + coordsRel.Z);
            return true;
        }
    }

    public bool SetBlock(out string blockName, Point3<int> blockCoordsRel, string[]? exclusions = null)
    {
        blockName = Block.AirBlockName;
        if (_blockPaletteIndexTable is null)
            return false;
        else
        {
            int blockTableIndex = _blockPaletteIndexTable[blockCoordsRel.X, blockCoordsRel.Y, blockCoordsRel.Z];
            Block blockTemplate = _blockPalette![blockTableIndex];
            // short circuit if air block encountered
            if (blockTemplate.Name == Block.AirBlockName
                || blockTemplate.Name == Block.AirCaveBlockName
                || exclusions is not null
                && exclusions.Contains(blockTemplate.Name))
                return false;
            blockName = blockTemplate.Name;
            return true;
        }
    }

    // long data stores what block is at xyz, and that block is corresponds
    // to one block from palette. The long data itself is in long data type form
    // and when being interpreted as binary, it can be broken into several sub-numbers,
    // holding many integers that stores pointers to palette index
    private static int[,,] ReadNbtLongData(NbtLongArray dataNbt, int paletteLength)
    {
        // bit-length required for single block id (minimum of 4) based from palette length.
        int blockBitLength = Math.Max((paletteLength - 1).Bitlength(), 4);

        int bitsInByte = 8;

        int longBitLength = sizeof(long) * bitsInByte; // sizeof unit is byte, convert to bit

        // maximum count of blocks can fit within single 'long' value
        int blockCount = longBitLength / blockBitLength;

        /* bit-length required for many blocks that can fit as much in 'long' bit-length (64-bit)
         * bit-length required to store block as many as block count
         * 
         * for example 6 bits required for single block, that means it can hold
         * at most 10 blocks: 6 bits * 10 blocks = 60 bits,
         * two more bits (become 66) are required to store one more block,
         * 4 bits are left redundant and discarded.
         * 
         * If for example the bit-length required for single block is 4 bits,
         * then it can hold at most 16 blocks: 4 bits * 16 blocks = 64 bits,
         * which in this case no single bit are left discarded, no wastage.
         */

        // 3D table, order is XZY
        int[,,] paletteIndexTable3D = new int[BlockCount, BlockCount, BlockCount];

        // pos is filling position to which index is to fill
        Point3<int> fillPos = Point3<int>.Zero;
#if DEBUG
        int longIndex = 0; // for easier debugging
#endif
        Span<int> buffer = stackalloc int[blockCount];

        bool breaking = false;
        foreach (long longValue in dataNbt.Values)
        {
            if (breaking)
                break;
#if UNSAFE_OPTIMIZATION
            BinaryUtils.SplitSubnumberFastNoCheck(longValue, buffer, blockBitLength);
#else
            BinaryUtils.SplitSubnumberFast(longValue, buffer, blockBitLength);
#endif
            foreach (int value in buffer)
            {
                if (breaking)
                    break;
                paletteIndexTable3D[fillPos.X, fillPos.Y, fillPos.Z] = value;
                if (fillPos.X < 15)
                    fillPos.X++;
                else
                {
                    fillPos.X = 0;
                    if (fillPos.Z < 15)
                        fillPos.Z++;
                    else
                    {
                        fillPos.Z = 0;
                        if (fillPos.Y < 15)
                            fillPos.Y++;
                        else
                            // if Y reached 15 and want to increment,
                            // it means filling is finished so we want to break
                            breaking = true;
                    }
                }
            }
#if DEBUG
            longIndex++;
#endif
        }
        return paletteIndexTable3D;
    }

    public override string ToString()
    {
        return $"Section {_sectionYPos} at {_sectionCoordsAbs}";
    }
}
