using System;
using System.Collections.Generic;

using binstarjs03.AerialOBJ.Core.Definitions;
using binstarjs03.AerialOBJ.Core.NbtFormat;
using binstarjs03.AerialOBJ.Core.Pooling;
using binstarjs03.AerialOBJ.Core.Primitives;

namespace binstarjs03.AerialOBJ.Core.MinecraftWorld;
public class Chunk2860 : IChunk, IDisposable
{
    private readonly Dictionary<int, Section> _sections;
    private readonly int[] _sectionsY;
    private bool _disposedValue;

    public Chunk2860(NbtCompound chunkNbt)
    {
        CoordsAbs = getChunkCoordsAbs(chunkNbt);
        CoordsRel = MinecraftWorldMathUtils.ConvertChunkCoordsAbsToRel(CoordsAbs);
        (_sections, _sectionsY) = readSections(chunkNbt);

        // just in case section elements is unsorted, it is most unlikely but yeah
        Array.Sort(_sectionsY);

        LowestBlockHeight = _sectionsY[0] * IChunk.BlockCount;
        HighestBlockHeight = _sectionsY[^1] * IChunk.BlockCount + IChunk.BlockCount;

        static PointZ<int> getChunkCoordsAbs(NbtCompound chunkNbt)
        {
            int chunkCoordsAbsX = chunkNbt.Get<NbtInt>("xPos").Value;
            int chunkCoordsAbsZ = chunkNbt.Get<NbtInt>("zPos").Value;
            return new PointZ<int>(chunkCoordsAbsX, chunkCoordsAbsZ);
        }

        (Dictionary<int, Section>, int[]) readSections(NbtCompound chunkNbt)
        {
            NbtList<NbtCompound> sectionsNbt = chunkNbt.Get<NbtList<NbtCompound>>("sections");
            int[] sectionsYPos = new int[sectionsNbt.Count];
            Dictionary<int, Section> sections = new();

            for (int i = 0; i < sectionsNbt.Count; i++)
            {
                NbtCompound sectionNbt = sectionsNbt[i];
                int sectionYPos = sectionNbt.Get<NbtByte>("Y").Value;
                Section section = new(sectionNbt)
                {
                    CoordsAbs = new Point3<int>(CoordsAbs.X, sectionYPos, CoordsAbs.Z)
                };

                sectionsYPos[i] = sectionYPos;
                sections.Add(sectionYPos, section);
            }

            return (sections, sectionsYPos);
        }
    }

    public PointZ<int> CoordsAbs { get; }
    public PointZ<int> CoordsRel { get; }
    public int DataVersion => 2860;
    public string MinecraftVersion => "1.18";
    public int LowestBlockHeight { get; }
    public int HighestBlockHeight { get; }

    public void GetHighestBlockSlim(ViewportDefinition vd, BlockSlim[,] highestBlockBuffer, int heightLimit)
    {
        for (int z = 0; z < IChunk.BlockCount; z++)
            for (int x = 0; x < IChunk.BlockCount; x++)
                highestBlockBuffer[x, z] = GetHighestBlockSlimSingleNoCheck(vd, new PointZ<int>(x, z), heightLimit, null);
    }

    public BlockSlim GetHighestBlockSlimSingleNoCheck(ViewportDefinition vd, PointZ<int> blockCoordsRel, int heightLimit, string? exclusion = null)
    {
        // scan block from top to bottom section
        for (int index = _sections.Count - 1; index >= 0; index--)
        {
            int sectionY = _sectionsY[index];
            Section section = _sections[sectionY];

            // skip if section is entirely filled with excluded blocks
            if (section.IsExcluded(vd))
                continue;

            // skip if section is higher than limit
            int heightAtSection = sectionY * IChunk.BlockCount;
            if (heightAtSection > heightLimit)
                continue;

            // scan block from top to bottom relative to section
            for (int y = IChunk.BlockRange; y >= 0; y--)
            {
                if (heightAtSection + y > heightLimit)
                    continue;

                Point3<int> blockCoordsRel3 = blockCoordsRel;
                blockCoordsRel3.Y = y;

                Block? paletteBlock = section.GetPaletteBlock(blockCoordsRel3);

                if (paletteBlock is null
                    || paletteBlock.Name.IsExcluded(vd)
                    || paletteBlock.Name == exclusion)
                    continue;

                return new BlockSlim(paletteBlock.Name, heightAtSection + y);
            }
        }

        // failed to get block in all sections and height ranges, set it to air at lowest level
        int lowestBlockY = LowestBlockHeight;
        return new BlockSlim(vd.AirBlockName, lowestBlockY);
    }

    public override string ToString()
    {
        return $"Chunk {CoordsAbs}, DataVersion: {DataVersion}";
    }

    private class Section : IDisposable
    {
        // TODO inject an instance of arraypool instead as static, this makes purging pool instances difficult if it is static!!!
        private static readonly ArrayPool3<int> s_blockPaletteIndexTablePooler = new(IChunk.BlockCount, IChunk.BlockCount, IChunk.BlockCount);
        private readonly int[,,]? _blockPaletteIndexTable;
        private readonly Block[]? _blockPalette;
        private bool _disposedValue;

        public Section(NbtCompound sectionNbt)
        {
            sectionNbt.TryGet("block_states", out NbtCompound? blockStatesNbt);
            if (blockStatesNbt is null)
                return;
            blockStatesNbt.TryGet("palette", out NbtList<NbtCompound>? paletteNbt);
            if (paletteNbt is null)
                return;

            _blockPalette = new Block[paletteNbt.Count];
            for (int i = 0; i < paletteNbt.Count; i++)
            {
                string blockName = paletteNbt[i].Get<NbtString>("Name").Value;
                _blockPalette[i] = new Block() { Name = blockName };
            }

            blockStatesNbt.TryGet("data", out NbtLongArray? dataNbt);
            if (dataNbt is null)
                return;
            _blockPaletteIndexTable = ReadNbtLongData(dataNbt, paletteNbt.Count);
        }

        public required Point3<int> CoordsAbs { get; init; }

        // whether section is completely filled with excluded block,
        // an optimization for finding highest, non-excluded block in chunk
        public bool IsExcluded(ViewportDefinition vd)
        {
            if (_blockPalette is null)
                return true;
            if (_blockPalette.Length == 1)
                return _blockPalette[0].Name.IsExcluded(vd);
            return false;
        }

        private int[,,]? ReadNbtLongData(NbtLongArray dataNbt, int paletteLength)
        {
            // 3D table, order is XZY
            int[,,] paletteIndexTable = s_blockPaletteIndexTablePooler.Rent();
            fillPaletteIndexTable(dataNbt, paletteIndexTable, paletteLength);
            return paletteIndexTable;

            static void fillPaletteIndexTable(NbtLongArray dataNbt, int[,,] paletteIndexTable, int paletteLength)
            {
                // bit-length required for single block
                // (minimum of 4) based from palette length.
                int blockBitLength = Math.Max((paletteLength - 1).Bitlength(), 4);
                int bitsInByte = 8;
                int longBitLength = sizeof(long) * bitsInByte;

                // maximum count of blocks that can fit within single 'long' value
                int blockCount = longBitLength / blockBitLength;

                Span<int> buffer = stackalloc int[blockCount];
                Point3<int> fillPos = Point3<int>.Zero;

                foreach (long packed in dataNbt.Values)
                {
                    BinaryUtils.UnpackBitNoCheck(packed, buffer, blockBitLength);
                    for (int i = 0; i < blockCount; i++)
                    {
                        /* we want to layout our index table in such way so it is
                         * optimized for getting highest block, where block is
                         * scanned from top to bottom, which in this case, Y is
                         * the rapidly-changing-index, and we want to minimize
                         * CPU cache miss so we put Y at the innermost order
                         */
                        paletteIndexTable[fillPos.Z, fillPos.X, fillPos.Y] = buffer[i];
                        if (moveFillingPosition(ref fillPos))
                            return;
                    }
                }
            }

            static bool moveFillingPosition(ref Point3<int> fillPos)
            {
                if (fillPos.X++ < 15)
                    return false;
                fillPos.X = 0;
                if (fillPos.Z++ < 15)
                    return false;
                fillPos.Z = 0;
                if (fillPos.Y++ < 15)
                    return false;
                return true;
            }
        }

        public Block? GetPaletteBlock(Point3<int> blockCoordsRel)
        {
            // if block palette is null, most likely the entire section is air
            if (_blockPalette is null)
                return null;

            // if block palette index table is null, it means the entire section
            // is filled with whatever block is in palette (most likely there is only one block)
            if (_blockPaletteIndexTable is null)
                return _blockPalette[0];

            int paletteIndex = _blockPaletteIndexTable[blockCoordsRel.Z,
                                                       blockCoordsRel.X,
                                                       blockCoordsRel.Y];
            return _blockPalette[paletteIndex];
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (_blockPaletteIndexTable is not null)
                    s_blockPaletteIndexTablePooler.Return(_blockPaletteIndexTable);
                _disposedValue = true;
            }
        }
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            foreach (var item in _sections.Values)
                item.Dispose();
            _disposedValue = true;
        }
    }

    void IDisposable.Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
