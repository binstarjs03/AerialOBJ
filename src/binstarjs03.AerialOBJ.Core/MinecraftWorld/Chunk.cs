/*
Minecraft World Parser Library

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

using binstarjs03.AerialOBJ.Core.CoordinateSystem;
using binstarjs03.AerialOBJ.Core.NbtNew;

namespace binstarjs03.AerialOBJ.Core.MinecraftWorld;

public class Chunk
{
    // TODO: these static fields are unreliable.
    // some custom worlds have their own range so we shouldn't rely on it
    public static readonly int TotalSectionCount = SectionRange.Max - SectionRange.Min;
    public static readonly Range SectionRange = new(-4, 19);
    public static readonly int TotalBlockCount = Section.TotalBlockCount * TotalSectionCount;
    public static readonly CoordsRange3 BlockRangeRel = new(
        xRange: new Range(0, Section.BlockCount),
        yRange: new Range(Section.BlockCount * SectionRange.Min, Section.BlockCount * SectionRange.Max + Section.BlockRange),
        zRange: new Range(0, Section.BlockCount)
    );

    private readonly Coords2 _chunkCoordsAbs;
    private readonly CoordsRange3 _blockRangeAbs;

    // array of section position or height
    private readonly int[] _sectionsYPos;

    // key is the position or height of the section.
    // if we are not sure, we can just index any sectionsYPos element
    private readonly Dictionary<int, Section> _sections;

    public Coords2 ChunkCoordsAbs => _chunkCoordsAbs;
    public CoordsRange3 BlockRangeAbs => _blockRangeAbs;

    public Chunk(NbtCompound chunkNbt)
    {
        _chunkCoordsAbs = calculateChunkCoordsAbs(chunkNbt);
        _blockRangeAbs = calculateBlockRangeAbs(_chunkCoordsAbs);
        (_sectionsYPos, _sections) = readSections(_chunkCoordsAbs, chunkNbt);

        // just in case section is unsorted, we sort them first
        Array.Sort(_sectionsYPos);

        static Coords2 calculateChunkCoordsAbs(NbtCompound chunkNbt)
        {
            int chunkCoordsAbsX = chunkNbt.Get<NbtInt>("xPos").Value;
            int chunkCoordsAbsZ = chunkNbt.Get<NbtInt>("zPos").Value;
            return new Coords2(chunkCoordsAbsX, chunkCoordsAbsZ);
        }
        static CoordsRange3 calculateBlockRangeAbs(Coords2 chunkCoordsAbs)
        {
            int blockRangeAbsMinX = chunkCoordsAbs.X * Section.BlockCount;
            int blockRangeAbsMinY = SectionRange.Min * Section.BlockCount;
            int blockRangeAbsMinZ = chunkCoordsAbs.Z * Section.BlockCount;
            Coords3 blockRangeAbsMin = new(blockRangeAbsMinX, blockRangeAbsMinY, blockRangeAbsMinZ);

            int blockRangeAbsMaxX = blockRangeAbsMinX + Section.BlockRange;
            int blockRangeAbsMaxY = SectionRange.Max * Section.BlockCount + Section.BlockRange;
            int blockRangeAbsMaxZ = blockRangeAbsMinZ + Section.BlockRange;
            Coords3 blockRangeAbsMax = new(blockRangeAbsMaxX, blockRangeAbsMaxY, blockRangeAbsMaxZ);

            return new CoordsRange3(blockRangeAbsMin, blockRangeAbsMax);
        }
        static (int[], Dictionary<int, Section>) readSections(Coords2 chunkCoordsAbs, NbtCompound chunkNbt)
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

    public static Coords3 ConvertBlockCoordsAbsToRel(Coords3 coords)
    {
        int blockCoordsAbsX = MathUtils.Mod(coords.X, Section.BlockCount);
        int blockCoordsAbsY = coords.Y;
        int blockCoordsAbsZ = MathUtils.Mod(coords.Z, Section.BlockCount);
        return new Coords3(blockCoordsAbsX, blockCoordsAbsY, blockCoordsAbsZ);
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

    public Block GetBlock(Coords3 coords, bool relative)
    {
        Coords3 coordsAbs;
        Coords3 coordsRel;
        Coords3 coordsLocalToSection;
        if (relative)
        {
            BlockRangeRel.ThrowIfOutside(coords);
            coordsAbs = new Coords3(_chunkCoordsAbs.X * Section.BlockCount + coords.X,
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

        coordsLocalToSection = new Coords3(coordsRel.X,
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

    public static string[,] GenerateHighestBlocksBuffer()
    {
        string[,] highestBlocks = new string[Section.BlockCount, Section.BlockCount];
        ReinitializeHighestBlocksBuffer(highestBlocks);
        return highestBlocks;
    }

    public void GetHighestBlock(string[,] highestBlocks, string[]? exclusions = null, int? heightLimit = null)
    {
        int limit = (int)(heightLimit is null ? int.MaxValue : heightLimit);

        // We expect the buffer is already initialized (should always do
        // if generated from GenerateHighestBlocksBuffer)

        // reinitialize to air
        //ReinitializeHighestBlocksBuffer(highestBlocks);

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

                        Coords3 blockCoordsRel = new(x, y, z);

                        // set existing block instance to avoid heap generation.
                        // generating heap at tight-loop like this will trash the GC very badly
                        // also SetBlock return true if setting is successful, 
                        // we want to break early if so since that is the highest block
                        breaking = section.SetBlock(out string blockName, blockCoordsRel, exclusions);
                        highestBlocks[x, z] = blockName;
                    }
                }
            }
    }

    public override string ToString()
    {
        return $"Chunk at {_chunkCoordsAbs}";
    }
}
