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
using System.Collections.Generic;

using binstarjs03.AerialOBJ.Core.Primitives;
using binstarjs03.AerialOBJ.Core.Nbt;

namespace binstarjs03.AerialOBJ.Core.MinecraftWorld;

public class Chunk
{
    // TODO: these static fields are unreliable.
    // some custom worlds have their own range so we shouldn't rely on it
    public static readonly int TotalSectionCount = SectionRange.Max - SectionRange.Min;
    public static readonly Rangeof<int> SectionRange = new(-4, 19);
    public static readonly int TotalBlockCount = Section.TotalBlockCount * TotalSectionCount;
    public static readonly Point3Range<int> BlockRangeRel = new(
        xRange: new Rangeof<int>(0, Section.BlockCount),
        yRange: new Rangeof<int>(Section.BlockCount * SectionRange.Min, Section.BlockCount * SectionRange.Max + Section.BlockRange),
        zRange: new Rangeof<int>(0, Section.BlockCount)
    );

    private readonly Point2Z<int> _chunkCoordsRel;
    private readonly Point2Z<int> _chunkCoordsAbs;
    private readonly Point3Range<int> _blockRangeAbs;

    // array of section position or height
    private readonly int[] _sectionsYPos;

    // key is the position or height of the section.
    // if we are not sure, we can just index any sectionsYPos element
    private readonly Dictionary<int, Section> _sections;

    public Point2Z<int> ChunkCoordsRel => _chunkCoordsRel;
    public Point2Z<int> ChunkCoordsAbs => _chunkCoordsAbs;
    public Point3Range<int> BlockRangeAbs => _blockRangeAbs;

    public Chunk(NbtCompound chunkNbt)
    {
        _chunkCoordsAbs = calculateChunkCoordsAbs(chunkNbt);
        _chunkCoordsRel = Region.ConvertChunkCoordsAbsToRel(_chunkCoordsAbs);
        _blockRangeAbs = calculateBlockRangeAbs(_chunkCoordsAbs);
        (_sectionsYPos, _sections) = readSections(_chunkCoordsAbs, chunkNbt);

        // just in case section is unsorted, we sort them first
        Array.Sort(_sectionsYPos);

        static Point2Z<int> calculateChunkCoordsAbs(NbtCompound chunkNbt)
        {
            int chunkCoordsAbsX = chunkNbt.Get<NbtInt>("xPos").Value;
            int chunkCoordsAbsZ = chunkNbt.Get<NbtInt>("zPos").Value;
            return new Point2Z<int>(chunkCoordsAbsX, chunkCoordsAbsZ);
        }
        static Point3Range<int> calculateBlockRangeAbs(Point2Z<int> chunkCoordsAbs)
        {
            int blockRangeAbsMinX = chunkCoordsAbs.X * Section.BlockCount;
            int blockRangeAbsMinY = SectionRange.Min * Section.BlockCount;
            int blockRangeAbsMinZ = chunkCoordsAbs.Z * Section.BlockCount;
            Point3<int> blockRangeAbsMin = new(blockRangeAbsMinX, blockRangeAbsMinY, blockRangeAbsMinZ);

            int blockRangeAbsMaxX = blockRangeAbsMinX + Section.BlockRange;
            int blockRangeAbsMaxY = SectionRange.Max * Section.BlockCount + Section.BlockRange;
            int blockRangeAbsMaxZ = blockRangeAbsMinZ + Section.BlockRange;
            Point3<int> blockRangeAbsMax = new(blockRangeAbsMaxX, blockRangeAbsMaxY, blockRangeAbsMaxZ);

            return new Point3Range<int>(blockRangeAbsMin, blockRangeAbsMax);
        }
        static (int[], Dictionary<int, Section>) readSections(Point2Z<int> chunkCoordsAbs, NbtCompound chunkNbt)
        {
            NbtList<NbtCompound> sectionsNbt = chunkNbt.Get<NbtList<NbtCompound>>("sections");

            int[] sectionsYPos = new int[sectionsNbt.Count];
            Dictionary<int, Section> sections = new();

            for (int i = 0; i < sectionsYPos.Length; i++)
            {
                NbtCompound sectionNbt = sectionsNbt[i];
                int sectionYPos = sectionNbt.Get<NbtByte>("Y").Value;
                Section section = new(chunkCoordsAbs, sectionNbt);

                sectionsYPos[i] = sectionYPos;
                sections.Add(sectionYPos, section);
            }

            return (sectionsYPos, sections);
        }
    }

    public static Point3<int> ConvertBlockCoordsAbsToRel(Point3<int> coords)
    {
        int blockCoordsAbsX = MathUtils.Mod(coords.X, Section.BlockCount);
        int blockCoordsAbsY = coords.Y;
        int blockCoordsAbsZ = MathUtils.Mod(coords.Z, Section.BlockCount);
        return new Point3<int>(blockCoordsAbsX, blockCoordsAbsY, blockCoordsAbsZ);
    }

    public static Point2Z<int> GetChunkCoordsAbsFromBlockCoordsAbs(Point2Z<int> blockCoordsAbs)
    {
        return new Point2Z<int>(MathUtils.DivFloor(blockCoordsAbs.X, Section.BlockCount),
                           MathUtils.DivFloor(blockCoordsAbs.Z, Section.BlockCount));
    }

    public int[] SectionsYPos => _sectionsYPos;

    public bool HasSection(int sectionYPos)
    {
        if (_sections.ContainsKey(sectionYPos))
            return true;
        return false;
    }

    public Section GetSection(int sectionYPos)
    {
        if (_sections.ContainsKey(sectionYPos))
            return _sections[sectionYPos];
        throw new KeyNotFoundException($"Section {sectionYPos} does not exist in chunk");
    }

    public Section GetSectionAt(int index)
    {
        int sectionPosition = _sectionsYPos[index];
        return _sections[sectionPosition];
    }

    public Section[] GetSections()
    {
        List<Section> sections = new();
        foreach (int sectionYPos in SectionsYPos)
        {
            Section section = GetSection(sectionYPos);
            sections.Add(section);
        }
        return sections.ToArray();
    }

    // avoid calling GetBlock on tight-loops, GC intensive!
    public Block GetBlock(Point3<int> coords, bool relative)
    {
        Point3<int> coordsAbs;
        Point3<int> coordsRel;
        Point3<int> coordsLocalToSection;
        if (relative)
        {
            BlockRangeRel.ThrowIfOutside(coords);
            coordsAbs = new Point3<int>(_chunkCoordsAbs.X * Section.BlockCount + coords.X,
                                     coords.Y,
                                    _chunkCoordsAbs.Z * Section.BlockCount + coords.Z);
            coordsRel = coords;
        }
        else // absolute
        {
            BlockRangeAbs.ThrowIfOutside(coords);
            coordsAbs = coords;
            coordsRel = ConvertBlockCoordsAbsToRel(coords);
        }

        coordsLocalToSection = new Point3<int>(coordsRel.X,
                                           MathUtils.Mod(coords.Y, Section.BlockCount),
                                           coordsRel.Z);

        int sectionYPos = (int)MathF.Floor(coords.Y / Section.BlockCount);
        if (HasSection(sectionYPos))
        {
            Section section = GetSection(sectionYPos);
            return section.GetBlock(coordsLocalToSection, relative: true);
        }
        else
            return new Block("minecraft:air", coordsAbs);
    }

    public static void ReinitializeHighestBlocksBuffer(string[,] highestBlocks)
    {
        for (int x = 0; x < Section.BlockCount; x++)
            for (int z = 0; z < Section.BlockCount; z++)
                highestBlocks[x, z] = Block.AirBlockName;
    }

    public void GetHighestBlock(ChunkHighestBlockInfo highestBlock, string[]? exclusions = null, int? heightLimit = null)
    {
        int limit = (int)(heightLimit is null ? int.MaxValue : heightLimit);

        // We expect the buffer is already initialized (should always do
        // if generated from GenerateHighestBlocksBuffer)

        for (int z = 0; z < Section.BlockCount; z++)
            for (int x = 0; x < Section.BlockCount; x++)
            {
                bool breaking = false;
                for (int index = _sectionsYPos.Length - 1; index >= 0; index--)
                {
                    if (breaking)
                        break;

                    int sectionPosition = _sectionsYPos[index];
                    Section section = _sections[sectionPosition];

                    // skip sections that is higher than heightLimit
                    int heightAtSection = section.SectionCoordsAbs.Y * Section.BlockCount;
                    if (heightAtSection > limit)
                        continue;

                    for (int y = Section.BlockRange; y >= 0; y--)
                    {
                        if (breaking)
                            break;

                        // we also want to skip blocks that is higher than height limit
                        int height = heightAtSection + y;
                        if (height > limit)
                            continue;

                        Point3<int> blockCoordsRel = new(x, y, z);

                        // set existing block instance to avoid heap generation.
                        // generating heap at tight-loop like this will trash the GC very badly
                        // also SetBlock return true if setting is successful, 
                        // we want to break early if so since that is the highest block
                        breaking = section.SetBlock(out string blockName, blockCoordsRel, exclusions);
                        highestBlock.Names[x, z] = blockName;
                        highestBlock.Heights[x, z] = height;
                    }
                }
            }
    }

    public override string ToString()
    {
        return $"Chunk at {_chunkCoordsAbs}";
    }
}
