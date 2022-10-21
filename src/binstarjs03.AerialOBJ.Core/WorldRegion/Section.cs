using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using binstarjs03.AerialOBJ.Core.CoordinateSystem;
using binstarjs03.AerialOBJ.Core.Nbt;

namespace binstarjs03.AerialOBJ.Core.WorldRegion;

public class Section
{
    public static readonly int BlockCount = 16;
    public static readonly int TotalBlockCount = (int)Math.Pow(BlockCount, 3);
    public static readonly int BlockRange = BlockCount - 1;
    public static readonly CoordsRange3 BlockRangeRel = new(
        min: Coords3.Zero,
        max: new Coords3(BlockRange, BlockRange, BlockRange)
    );

    private readonly int _yPos;
    private readonly Coords3 _coordsAbs;
    private readonly CoordsRange3 _blockRangeAbs;

    private readonly int[,,]? _blockPaletteIndexTable;
    private readonly Block[]? _blockTable;

    public Section(Coords2 chunkCoordsAbs, NbtCompound sectionNbt)
    {
        _yPos = sectionNbt.Get<NbtByte>("Y").Value;
        _coordsAbs = calculateCoordsAbs(chunkCoordsAbs, _yPos);
        _blockRangeAbs = calculateBlockRangeAbs(_coordsAbs);

        if (sectionNbt.HasTag("block_states"))
        {
            if (!sectionNbt.HasTag("block_states"))
                return;
            NbtCompound blockStatesNbt = sectionNbt.Get<NbtCompound>("block_states");
            NbtList? paletteBlockNbt = readPaletteNbt(blockStatesNbt);

            if (paletteBlockNbt is null)
                return;

            _blockTable = new Block[paletteBlockNbt.Length];
            for (int i = 0; i < paletteBlockNbt.Length; i++)
            {
                NbtCompound blockPropertyNbt = paletteBlockNbt.Get<NbtCompound>(i);
                // block template in block table coordinate doesn't matter
                // so we set it to zero
                _blockTable[i] = new Block(Coords3.Zero, blockPropertyNbt);
            }

            NbtArrayLong? dataNbt = readDataNbt(blockStatesNbt);
            if (dataNbt is not null)
                _blockPaletteIndexTable = ReadNbtLongData(dataNbt, paletteBlockNbt.Length);
        }
        else
            return;

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
        static NbtList? readPaletteNbt(NbtCompound nbtBlockStates)
        {
            if (!nbtBlockStates.HasTag("palette"))
                return null;
            return nbtBlockStates.Get<NbtList>("palette");
        }
        static NbtArrayLong? readDataNbt(NbtCompound nbtBlockStates)
        {
            if (!nbtBlockStates.HasTag("data"))
                return null;
            return nbtBlockStates.Get<NbtArrayLong>("data");
        }
    }

    public Coords3 CoordsAbs => _coordsAbs;

    public CoordsRange3 BlockRangeAbs => _blockRangeAbs;

    public static Coords3 ConvertBlockAbsToRel(Coords3 coords)
    {
        int relBx = MathUtils.Mod(coords.X, BlockCount);
        int relBy = MathUtils.Mod(coords.Y, BlockCount);
        int relBz = MathUtils.Mod(coords.Z, BlockCount);
        return new Coords3(relBx, relBy, relBz);
    }

    public Block GetBlock(Coords3 coords, bool relative)
    {
        Coords3 coordsRel;
        Coords3 coordsAbs;
        if (relative)
        {
            BlockRangeRel.ThrowIfOutside(coords);
            coordsRel = coords;
            coordsAbs = new Coords3(_coordsAbs.X * BlockCount + coords.X,
                                    _coordsAbs.Y * BlockCount + coords.Y,
                                    _coordsAbs.Z * BlockCount + coords.Z);
        }
        else
        {
            BlockRangeAbs.ThrowIfOutside(coords);
            coordsRel = ConvertBlockAbsToRel(coords);
            coordsAbs = coords;
        }
        if (_blockPaletteIndexTable is null)
            return new Block(coordsAbs); // return air block
        else
        {
            int blockTableIndex = _blockPaletteIndexTable[coordsRel.X, coordsRel.Y, coordsRel.Z];
            Block block = _blockTable![blockTableIndex].Clone();
            block.CoordsAbs = coordsAbs;
            return block;
        }
    }

    // long data stores what block is at xyz, and that block is corresponds
    // to one block from palette. The long data itself is in long data type form
    // and when being interpreted as binary, it holds many integers that stores
    // exactly what block is at xyz.
    private static int[,,] ReadNbtLongData(NbtArrayLong dataNbt, int paletteLength)
    {
        // bit-length required for single block id (minimum of 4) based from palette length.
        int blockBitLength = Math.Max((paletteLength - 1).Bitlength(), 4);

        int bitsInByte = 8;
        int longBitLength = sizeof(long) * bitsInByte; // sizeof unit is byte, convert to bit

        // maximum count of blocks can fit within single 'long' value
        int blockCount = (int)MathF.Floor(longBitLength / blockBitLength);

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

        // linear table, order is XZY **
        List<int> paletteIndexTable = new(TotalBlockCount);
        foreach (long binInLongForm in dataNbt.Values)
        {
            // extract binary from long and reverse it.
            // This makes the data bit order in little-endian.
            List<int> intTableFragment = new(blockCount);
            byte[] bin = binInLongForm.ToBinaryArray(longBitLength).Reverse().ToArray();
            using (MemoryStream binStream = new(bin))
            using (BinaryReader binReader = new(binStream))
            {
                for (int i = 0; i < blockCount; i++)
                {
                    // do not add remaining redundant data to block-map-palette table
                    // if the element count reached maximum blocks count of section (16*16*16 = 4096)
                    if (paletteIndexTable.Count == TotalBlockCount)
                        break;

                    // bin buffer for current, single block id.
                    byte[] buff = binReader.ReadBytes(blockBitLength);

                    // convert bin to int, this will return us the block palette index
                    // for current block
                    int blockId = buff.ToIntLE();
                    intTableFragment.Add(blockId);
                }
            }
            paletteIndexTable.AddRange(intTableFragment);
        }

        // transform linear table (1D) becoming 3D table
        int[,,] paletteIndexTable3D = new int[BlockCount, BlockCount, BlockCount];
        for (int x = 0; x < BlockCount; x++)
            for (int y = 0; y < BlockCount; y++)
                for (int z = 0; z < BlockCount; z++)
                {
                    // map 3D array idx to linear array idx, see ** above
                    int linearIndex = x
                                    + z * (int)Math.Pow(BlockCount, 1)
                                    + y * (int)Math.Pow(BlockCount, 2);
                    paletteIndexTable3D[x, y, z] = paletteIndexTable[linearIndex];
                }

        return paletteIndexTable3D;
    }

    public override string ToString()
    {
        return $"Section {_yPos} of {_coordsAbs}";
    }
}
