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

using binstarjs03.AerialOBJ.Core.CoordinateSystem;
using binstarjs03.AerialOBJ.Core.Nbt;

namespace binstarjs03.AerialOBJ.Core.MinecraftWorld;

public class Section
{
    public const int BlockCount = 16;
    public const int TotalBlockCount = BlockCount * BlockCount * BlockCount;
    public const int BlockRange = BlockCount - 1;
    public static readonly CoordsRange3 BlockRangeRel = new(
        min: Coords3.Zero,
        max: new Coords3(BlockRange, BlockRange, BlockRange)
    );

    private readonly int _sectionYPos;
    private readonly Coords3 _sectionCoordsAbs;
    private readonly CoordsRange3 _blockRangeAbs;

    private readonly int[,,]? _blockPaletteIndexTable;
    private readonly Block[]? _blockPalette;

    public Coords3 SectionCoordsAbs => _sectionCoordsAbs;
    public CoordsRange3 BlockRangeAbs => _blockRangeAbs;

    public Section(Coords2 chunkCoordsAbs, NbtCompound sectionNbt)
    {
        _sectionYPos = sectionNbt.Get<NbtByte>("Y").Value;
        _sectionCoordsAbs = calculateCoordsAbs(chunkCoordsAbs, _sectionYPos);
        _blockRangeAbs = calculateBlockRangeAbs(_sectionCoordsAbs);

        if (!sectionNbt.ContainsKey("block_states"))
            return;
        NbtCompound blockStatesNbt = sectionNbt.Get<NbtCompound>("block_states");
        NbtList<NbtCompound>? paletteBlockNbt = readPaletteNbt(blockStatesNbt);

        if (paletteBlockNbt is null)
            return;

        _blockPalette = new Block[paletteBlockNbt.Count];
        for (int i = 0; i < paletteBlockNbt.Count; i++)
        {
            NbtCompound blockPropertyNbt = paletteBlockNbt[i];
            // block template in block table coordinate doesn't matter
            // so we set it to zero
            _blockPalette[i] = new Block(Coords3.Zero, blockPropertyNbt);
        }

        NbtLongArray? dataNbt = readDataNbt(blockStatesNbt);
        if (dataNbt is not null)
            _blockPaletteIndexTable = ReadNbtLongData(dataNbt, paletteBlockNbt.Count);

        static Coords3 calculateCoordsAbs(Coords2 chunkCoordsAbs, int yPos)
        {
            int x = chunkCoordsAbs.X;
            int y = yPos;
            int z = chunkCoordsAbs.Z;
            return new Coords3(x, y, z);
        }
        static CoordsRange3 calculateBlockRangeAbs(Coords3 coordsAbs)
        {
            int minAbsBx = coordsAbs.X * BlockCount;
            int minAbsBy = coordsAbs.Y * BlockCount;
            int minAbsBz = coordsAbs.Z * BlockCount;
            Coords3 minAbsB = new(minAbsBx, minAbsBy, minAbsBz);

            int maxAbsBx = minAbsBx + BlockRange;
            int maxAbsBy = minAbsBy + BlockRange;
            int maxAbsBz = minAbsBz + BlockRange;
            Coords3 maxAbsB = new(maxAbsBx, maxAbsBy, maxAbsBz);

            return new CoordsRange3(minAbsB, maxAbsB);
        }
        static NbtList<NbtCompound>? readPaletteNbt(NbtCompound nbtBlockStates)
        {
            if (nbtBlockStates.ContainsKey("palette"))
                return nbtBlockStates.Get<NbtList<NbtCompound>>("palette");
            return null;
        }
        static NbtLongArray? readDataNbt(NbtCompound nbtBlockStates)
        {
            if (nbtBlockStates.ContainsKey("data"))
                return nbtBlockStates.Get<NbtLongArray>("data");
            return null;
        }
    }

    public static Coords3 ConvertBlockCoordsAbsToRel(Coords3 coords)
    {
        int blockCoordsAbsX = MathUtils.Mod(coords.X, BlockCount);
        int blockCoordsAbsY = MathUtils.Mod(coords.Y, BlockCount);
        int blockCoordsAbsZ = MathUtils.Mod(coords.Z, BlockCount);
        return new Coords3(blockCoordsAbsX, blockCoordsAbsY, blockCoordsAbsZ);
    }

    public (Coords3 blockCoordsRel, Coords3 blockCoordsAbs) CalculateBlockCoords(Coords3 blockCoords, bool relative, bool skipTest = false)
    {
        Coords3 blockCoordsRel;
        Coords3 blockCoordsAbs;
        if (relative)
        {
            if (!skipTest)
                BlockRangeRel.ThrowIfOutside(blockCoords);
            blockCoordsRel = blockCoords;
            blockCoordsAbs = new Coords3(_sectionCoordsAbs.X * BlockCount + blockCoords.X,
                                         _sectionCoordsAbs.Y * BlockCount + blockCoords.Y,
                                         _sectionCoordsAbs.Z * BlockCount + blockCoords.Z);
        }
        else
        {
            if (!skipTest)
                BlockRangeAbs.ThrowIfOutside(blockCoords);
            blockCoordsRel = ConvertBlockCoordsAbsToRel(blockCoords);
            blockCoordsAbs = blockCoords;
        }
        return (blockCoordsRel, blockCoordsAbs);
    }

    // get what block the block table returned from given input.
    // This method does generate heap so avoid calling this method at tight-loops
    public Block GetBlock(Coords3 coords, bool relative)
    {
        (Coords3 coordsRel, Coords3 coordsAbs) = CalculateBlockCoords(coords, relative);
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
            Block block = new(blockPalette.Name, blockPalette.BlockCoordsAbs);
            return block;
        }
    }

    // peek what the block table returned from given input.
    // does not generate heap since it just referencing palette block
    public Block GetBlockPalette(Coords3 blockCoordsRel)
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
    public bool SetBlock(Block block, Coords3 coordsRel, string[]? exclusions = null)
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
            block.BlockCoordsAbs = new Coords3(SectionCoordsAbs.X * BlockCount + coordsRel.X,
                                          SectionCoordsAbs.Y * BlockCount + coordsRel.Y,
                                          SectionCoordsAbs.Z * BlockCount + coordsRel.Z);
            block.Properties = blockTemplate.Properties;
            return true;
        }
    }

    public bool SetBlock(out string blockName, Coords3 blockCoordsRel, string[]? exclusions = null)
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
        Coords3 fillPos = Coords3.Zero;
        // int longIndex = 0; this was added before for easier debugging

        int[] buffer = new int[blockCount];

        bool breaking = false;
        foreach (long longValue in dataNbt.Values)
        {
            if (breaking)
                break;
            BinaryUtils.SplitSubnumberFastNoCheck(longValue, buffer, blockBitLength);
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
            // longIndex++;
        }
        return paletteIndexTable3D;
    }

    public override string ToString()
    {
        return $"Section {_sectionYPos} at {_sectionCoordsAbs}";
    }
}
