using System;
using System.Collections.Generic;
using System.Linq;

using binstarjs03.AerialOBJ.Core.CoordinateSystem;
using binstarjs03.AerialOBJ.Core.Nbt;

namespace binstarjs03.AerialOBJ.Core.WorldRegion;

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

    private readonly Coords2 _coordsAbs;
    private readonly CoordsRange3 _blockRangeAbs;

    // array of section position or height
    private readonly int[] _sectionsYPos;

    // key is the position or height of the section.
    // if we are not sure, we can just index any sectionsYPos element
    private readonly Dictionary<int, Section> _sections;

    public Chunk(NbtCompound nbtChunk)
    {
        _coordsAbs = calculateCoordsAbs(nbtChunk);
        _blockRangeAbs = calculateBlockRangeAbs(_coordsAbs);
        (_sectionsYPos, _sections) = readSections(_coordsAbs, nbtChunk);

        // just in case section is unordered, we order them first
        Array.Sort(_sectionsYPos);

        static Coords2 calculateCoordsAbs(NbtCompound nbtChunk)
        {
            int x = nbtChunk.Get<NbtInt>("xPos").Value;
            int z = nbtChunk.Get<NbtInt>("zPos").Value;
            return new Coords2(x, z);
        }
        static CoordsRange3 calculateBlockRangeAbs(Coords2 coordsAbs)
        {
            int minAbsBx = coordsAbs.X * Section.BlockCount;
            int minAbsBy = SectionRange.Min * Section.BlockCount;
            int minAbsBz = coordsAbs.Z * Section.BlockCount;
            Coords3 minAbsB = new(minAbsBx, minAbsBy, minAbsBz);

            int maxAbsBx = minAbsBx + Section.BlockRange;
            int maxAbsBy = SectionRange.Max * Section.BlockCount + Section.BlockRange;
            int maxAbsBz = minAbsBz + Section.BlockRange;
            Coords3 maxAbsB = new(maxAbsBx, maxAbsBy, maxAbsBz);

            return new CoordsRange3(minAbsB, maxAbsB);
        }
        static (int[], Dictionary<int, Section>) readSections(Coords2 chunkCoordsAbs, NbtCompound nbtChunk)
        {
            NbtList sectionsNbt = nbtChunk.Get<NbtList>("sections");
            int sectionLength = sectionsNbt.Length;

            int[] sectionsYPos = new int[sectionsNbt.Length];
            Dictionary<int, Section> sections = new();

            for (int i = 0; i < sectionLength; i++)
            {
                NbtCompound sectionNbt = sectionsNbt.Get<NbtCompound>(i);
                int sectionYPos = sectionNbt.Get<NbtByte>("Y").Value;
                Section section = new(chunkCoordsAbs, sectionNbt);

                sectionsYPos[i] = sectionYPos;
                sections.Add(sectionYPos, section);
            }
            return (sectionsYPos, sections);
        }
    }

    public Coords2 CoordsAbs => _coordsAbs;

    public CoordsRange3 BlockRangeAbs => _blockRangeAbs;

    public int[] SectionsYPos => _sectionsYPos;

    public bool HasSection(int sectionY)
    {
        if (_sections.ContainsKey(sectionY))
            return true;
        return false;
    }

    public Section GetSection(int sectionY)
    {
        if (!_sections.ContainsKey(sectionY))
            throw new KeyNotFoundException($"Section {sectionY} does not exist in chunk");
        return _sections[sectionY];
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
            coordsAbs = new Coords3(_coordsAbs.X * Section.BlockCount + coords.X,
                                     coords.Y,
                                    _coordsAbs.Z * Section.BlockCount + coords.Z);
            coordsRel = coords;
        }
        else // absolute
        {
            BlockRangeAbs.ThrowIfOutside(coords);
            coordsAbs = coords;
            coordsRel = ConvertBlockAbsToRel(coords);
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

    public static string[,] GenerateHighestBlocksBuffer()
    {
        string[,] highestBlocks = new string[Section.BlockCount, Section.BlockCount];
        for (int x = 0; x < Section.BlockCount; x++)
            for (int z = 0; z < Section.BlockCount; z++)
                highestBlocks[x, z] = Block.AirBlockName;
        return highestBlocks;
    }

    public void GetHighestBlock(string[,] highestBlocks, string[]? exclusions = null, int? heightLimit = null)
    {
        int limit = (int)(heightLimit is null ? int.MaxValue : heightLimit);

        // We expect the buffer is already initialized

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
                    int heightAtSection = section.CoordsAbs.Y * Section.BlockCount;
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

                        // get reference to current block
                        Coords3 coordsRel = new(x, y, z);

                        // set existing block instance to avoid heap generation.
                        // generating heap at tight-loop like this will trash the GC very badly
                        // also SetBlock return true if setting is successful, 
                        // we want to break early if so since that is the highest block
                        breaking = section.SetBlock(out string blockName, coordsRel, exclusions);
                        highestBlocks[x, z] = blockName;
                    }
                }
            }
    }

    public static Coords3 ConvertBlockAbsToRel(Coords3 coords)
    {
        int relBx = MathUtils.Mod(coords.X, Section.BlockCount);
        int relBy = coords.Y;
        int relBz = MathUtils.Mod(coords.Z, Section.BlockCount);
        return new Coords3(relBx, relBy, relBz);
    }

    public override string ToString()
    {
        return $"Chunk {_coordsAbs}";
    }
}
