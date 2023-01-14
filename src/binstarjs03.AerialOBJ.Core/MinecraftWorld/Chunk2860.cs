using System;
using System.Collections.Generic;

using binstarjs03.AerialOBJ.Core.Nbt;
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
        Array.Sort(_sectionsY);

        static Point2Z<int> getChunkCoordsAbs(NbtCompound chunkNbt)
        {
            int chunkCoordsAbsX = chunkNbt.Get<NbtInt>("xPos").Value;
            int chunkCoordsAbsZ = chunkNbt.Get<NbtInt>("zPos").Value;
            return new Point2Z<int>(chunkCoordsAbsX, chunkCoordsAbsZ);
        }
        (Dictionary<int, Section>, int[]) readSections(NbtCompound chunkNbt)
        {
            NbtList<NbtCompound> sectionsNbt = chunkNbt.Get<NbtList<NbtCompound>>("sections");
            int[] sectionsYPos = new int[sectionsNbt.Count];
            Dictionary<int, Section> sections = new();

            for (int i = 0; i < sectionsYPos.Length; i++)
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

    public Point2Z<int> CoordsAbs { get; }
    public Point2Z<int> CoordsRel { get; }
    public int DataVersion => 2860;
    public string ReleaseVersion => "1.18";

    public void GetHighestBlock(Block[,] highestBlockBuffer, int heightLimit)
    {
        for (int z = 0; z < IChunk.BlockCount; z++)
            for (int x = 0; x < IChunk.BlockCount; x++)
            {
                // assign initial highest block to air of lowest section
                Section lowestSection = _sections[0];
                int lowestBlock = lowestSection.CoordsAbs.Y * IChunk.BlockCount;
                highestBlockBuffer[x, z] = new Block()
                {
                    Coords = new Point3<int>(x, lowestBlock, z),
                    Name = "minecraft:air"
                };

                bool foundHighestBlock = false;
                for (int index = _sectionsY.Length - 1; index >= 0; index--)
                {
                    if (foundHighestBlock)
                        break;

                    int sectionY = _sectionsY[index];

                    int heightAtSection = sectionY * IChunk.BlockCount;
                    if (heightAtSection > heightLimit)
                        continue;

                    Section section = _sections[sectionY];
                    if (section.IsAir)
                        continue;

                    for (int y = IChunk.BlockRange; y >= 0; y--)
                    {
                        if (foundHighestBlock)
                            break;

                        if (heightAtSection + y > heightLimit)
                            continue;

                        Point3<int> blockCoordsRel = new(x, y, z);
                        Block? block = section.GetBlock(blockCoordsRel);
                        if (block is null || block.Value.IsAir)
                            continue;

                        highestBlockBuffer[x, z] = block.Value;
                        foundHighestBlock = true;
                    }
                }
            }
    }

    private class Section : IDisposable
    {
        private static readonly ArrayPool3<int> _blockPaletteIndexTablePooler = new(IChunk.BlockCount, IChunk.BlockCount, IChunk.BlockCount);
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
                _blockPalette[i] = new Block()
                {
                    Name = blockName,
                    Coords = Point3<int>.Zero
                };
            }

            blockStatesNbt.TryGet("data", out NbtLongArray? dataNbt);
            if (dataNbt is null)
                return;
            _blockPaletteIndexTable = ReadNbtLongData(dataNbt, paletteNbt.Count);
        }

        public required Point3<int> CoordsAbs { get; init; }
        public bool IsAir // whether section is completely filled with air block
        {
            get
            {
                if (_blockPalette is null)
                    return true;
                if (_blockPalette.Length == 1 && _blockPalette[0].IsAir)
                    return true;
                return false;
            }
        }

        private int[,,]? ReadNbtLongData(NbtLongArray dataNbt, int paletteLength)
        {
            // bit-length required for single block id
            // (minimum of 4) based from palette length.
            int blockBitLength = Math.Max((paletteLength - 1).Bitlength(), 4);
            int bitsInByte = 8;
            int longBitLength = sizeof(long) * bitsInByte;

            // maximum count of blocks can fit within single 'long' value
            int blockCount = longBitLength / blockBitLength;

            // 3D table, order is XZY
            int[,,] paletteIndexTable3D = _blockPaletteIndexTablePooler.Rent();

            // filling position to which index is to fill
            Point3<int> fillPos = Point3<int>.Zero;

            Span<int> buffer = stackalloc int[blockCount];

            bool filledCompletely = false;
            foreach (long longValue in dataNbt.Values)
            {
                if (filledCompletely)
                    break;
                BinaryUtils.SplitSubnumberFastNoCheck(longValue, buffer, blockBitLength);
                foreach (int value in buffer)
                {
                    if (filledCompletely)
                        break;
                    paletteIndexTable3D[fillPos.X, fillPos.Y, fillPos.Z] = value;
                    filledCompletely = moveFillingPosition(ref fillPos);
                }
            }
            return paletteIndexTable3D;

            // returns true when filled completely
            // if Y reached 15 and want to increment, it means filling is finished
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

        public Block? GetBlock(Point3<int> blockCoordsRel)
        {
            if (_blockPalette is null || _blockPaletteIndexTable is null)
                return null;
            int paletteIndex = _blockPaletteIndexTable[blockCoordsRel.X,
                                                       blockCoordsRel.Y,
                                                       blockCoordsRel.Z];
            // avoid creating new block, we just need to access the name
            ref Block blockPalette = ref _blockPalette[paletteIndex];
            return new Block()
            {
                Coords = MinecraftWorldMathUtils.ConvertBlockCoordsRelToSectionToAbs(blockCoordsRel, CoordsAbs),
                Name = blockPalette.Name,
            };
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (_blockPaletteIndexTable is not null)
                    _blockPaletteIndexTablePooler.Return(_blockPaletteIndexTable);
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
