using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using binstarjs03.MineSharpOBJ.Core.Utils;
using binstarjs03.MineSharpOBJ.Core.Nbt.Concrete;
namespace binstarjs03.MineSharpOBJ.Core.Region;


public class Section {
    public static readonly int BlockCount = 16;
    public static readonly int TotalBlockCount = (int)Math.Pow(BlockCount, 3);
    public static readonly int BlockRange = BlockCount - 1;
    public static readonly Coords3Range BlockRangeRel = new(
        min: Coords3.Origin,
        max: new Coords3(BlockRange, BlockRange, BlockRange)
    );

    private readonly NbtCompound _nbtSection;
    private readonly int _yPos;
    private readonly Coords3 _coordsRel;
    private readonly Coords3 _coordsAbs;
    private readonly Coords3Range _blockRangeAbs;
    private readonly NbtCompound? _nbtBlockStates;
    private readonly NbtList? _nbtPalette;
    private readonly NbtArrayLong? _nbtData;
    private int[]? _paletteIndexTable;

    public Section(Chunk chunk, NbtCompound nbtSection) {
        _nbtSection = nbtSection;
        _yPos = nbtSection.Get<NbtByte>("Y").Value;
        _coordsRel = evaluateCoordsRel(chunk, _yPos);
        _coordsAbs = evaluateCoordsAbs(chunk, _yPos);
        _blockRangeAbs = evaluateBlockRangeAbs(_coordsAbs);
        _nbtBlockStates = nbtSection.Get<NbtCompound>("block_states");
        _nbtPalette = initNbtPalette(_nbtBlockStates);
        _nbtData = initNbtData(_nbtBlockStates);

        static Coords3 evaluateCoordsRel(Chunk chunk, int yPos) {
            int x = chunk.CoordsRel.x;
            int y = yPos;
            int z = chunk.CoordsRel.z;
            return new Coords3(x, y, z);
        }
        static Coords3 evaluateCoordsAbs(Chunk chunk, int yPos) {
            int x = chunk.CoordsAbs.x;
            int y = yPos;
            int z = chunk.CoordsAbs.z;
            return new Coords3(x, y, z);
        }
        static Coords3Range evaluateBlockRangeAbs(Coords3 coordsAbs) {
            int minAbsBx = coordsAbs.x * BlockCount;
            int minAbsBy = coordsAbs.y * BlockCount;
            int minAbsBz = coordsAbs.z * BlockCount;
            Coords3 minAbsB = new(minAbsBx, minAbsBy, minAbsBz);

            int maxAbsBx = minAbsBx + BlockRange;
            int maxAbsBy = minAbsBy + BlockRange;
            int maxAbsBz = minAbsBz + BlockRange;
            Coords3 maxAbsB = new(maxAbsBx, maxAbsBy, maxAbsBz);

            return new Coords3Range(minAbsB, maxAbsB);
        }
        static NbtList? initNbtPalette(NbtCompound nbtBlockStates) {
            if (!nbtBlockStates.HasTag("palette"))
                return null;
            return nbtBlockStates.Get<NbtList>("palette");
        }
        static NbtArrayLong? initNbtData(NbtCompound nbtBlockStates) {
            if (!nbtBlockStates.HasTag("data"))
                return null;
            return nbtBlockStates.Get<NbtArrayLong>("data");
        }
    }

    public Coords3 CoordsRel => _coordsRel;

    public Coords3 CoordsAbs => _coordsAbs;

    public Coords3Range BlockRangeAbs => _blockRangeAbs;

    public NbtCompound NbtSection => _nbtSection;

    public static Coords3 ConvertBlockAbsToRel(Coords3 coords) {
        int relBx = MathUtils.Mod(coords.x, BlockCount);
        int relBy = MathUtils.Mod(coords.y, BlockCount);
        int relBz = MathUtils.Mod(coords.z, BlockCount);
        return new Coords3(relBx, relBy, relBz);
    }

    public Block GetBlock(Coords3 coords, bool relative) {
        if (relative)
            BlockRangeRel.IsOutside(coords);
        else {
            BlockRangeAbs.IsOutside(coords);
            coords = ConvertBlockAbsToRel(coords);
        }
        if (_nbtBlockStates is null)
            return new Block(this, coords);
        else
            return GetBlockFromPalette(coords);
    }

    private Block GetBlockFromPalette(Coords3 coords) {
        NbtCompound nbtBlockProperties;
        if (_nbtPalette is null)
            throw new NullReferenceException();
        if (_nbtData is null) {
            if (_nbtPalette.Length != 1)
                throw new InvalidOperationException();
            else
                nbtBlockProperties = _nbtPalette.Get<NbtCompound>(0);
        }
        else {
            int paletteIndex = GetPaletteIndex(coords);
            nbtBlockProperties = _nbtPalette.Get<NbtCompound>(paletteIndex);
        }
        return new Block(this, coords, nbtBlockProperties);
    }

    private int GetPaletteIndex(Coords3 relcoords) {
        int linearIndex = relcoords.x // map 3D array idx to linear array idx
                        + relcoords.z * (int)Math.Pow(BlockCount, 1)
                        + relcoords.y * (int)Math.Pow(BlockCount, 2);

        _paletteIndexTable ??= ReadNbtLongData(); // if null, invoke
        int paletteIndex = _paletteIndexTable[linearIndex];
        return paletteIndex;
    }

    private int[] ReadNbtLongData() {
        if (_nbtPalette is null || _nbtData is null)
            throw new NullReferenceException();
        int paletteLength = _nbtPalette.Length;

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
        //int paddedLongBitLength = blockCount * blockBitLength;

        List<int> paletteIndexTable = new(TotalBlockCount);
        foreach (long binInLongForm in _nbtData.Values) {
            // extract binary from long and reverse it.
            // This makes the data bit order in little-endian.
            List<int> intTableFragment = new(blockCount);
            byte[] bin = binInLongForm.ToBinaryArray(longBitLength).Reverse().ToArray();
            using (MemoryStream binStream = new(bin))
            using (BinaryReader binReader = new(binStream)) {
                for (int i = 0; i < blockCount; i++) {
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
        return paletteIndexTable.ToArray();
    }

}
