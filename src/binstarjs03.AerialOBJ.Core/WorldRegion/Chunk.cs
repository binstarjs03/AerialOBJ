using System;
using System.Collections.Generic;
using System.Linq;

using binstarjs03.AerialOBJ.Core.CoordinateSystem;
using binstarjs03.AerialOBJ.Core.Nbt;

namespace binstarjs03.AerialOBJ.Core.WorldRegion;

public class Chunk
{
    // TODO: these static fields are unreliable. some custom worlds have their own range so we have to refactor the class
    public static readonly int TotalSectionCount = SectionRange.max - SectionRange.min;
    public static readonly (int min, int max) SectionRange = (-4, 19);
    public static readonly int TotalBlockCount = Section.TotalBlockCount * TotalSectionCount;
    public static readonly CoordsRange3 BlockRangeRel = new(
        xRange: new Range(0, Section.BlockCount),
        yRange: new Range(Section.BlockCount * SectionRange.min, Section.BlockCount * SectionRange.max + Section.BlockRange),
        zRange: new Range(0, Section.BlockCount)
    );

    private readonly Coords2 _coordsAbs;
    private readonly CoordsRange3 _blockRangeAbs;
    private readonly int[] _sectionsYPos;

    private readonly Dictionary<int, Section> _sections;

    public Chunk(NbtCompound nbtChunk)
    {
        _coordsAbs = calculateCoordsAbs(nbtChunk);
        _blockRangeAbs = calculateBlockRangeAbs(_coordsAbs);
        (_sectionsYPos, _sections) = readSections(_coordsAbs, nbtChunk);

        static Coords2 calculateCoordsAbs(NbtCompound nbtChunk)
        {
            int x = nbtChunk.Get<NbtInt>("xPos").Value;
            int z = nbtChunk.Get<NbtInt>("zPos").Value;
            return new Coords2(x, z);
        }
        static CoordsRange3 calculateBlockRangeAbs(Coords2 coordsAbs)
        {
            int minAbsBx = coordsAbs.X * Section.BlockCount;
            int minAbsBy = SectionRange.min * Section.BlockCount;
            int minAbsBz = coordsAbs.Z * Section.BlockCount;
            Coords3 minAbsB = new(minAbsBx, minAbsBy, minAbsBz);

            int maxAbsBx = minAbsBx + Section.BlockRange;
            int maxAbsBy = SectionRange.max * Section.BlockCount + Section.BlockRange;
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

    // TODO need more polishing, especially if height limit value is invalid
    public Block[,] GetBlockTopmost(string[] exclusions, int? heightLimit = null)
    {
        int limit = (int)(heightLimit is null ? int.MaxValue : heightLimit);
        Block[,] blocks = new Block[Section.BlockCount, Section.BlockCount];
        Block block;
        Coords3 coords;

        // set initial block, which is air
        for (int x = 0; x < Section.BlockCount; x++)
        {
            for (int z = 0; z < Section.BlockCount; z++)
            {
                coords = new(_coordsAbs.X * Section.BlockCount + x,
                                     0,
                                     _coordsAbs.Z * Section.BlockCount + z);
                Block air = new(coords);
                blocks[x, z] = air;
            }
        }

        for (int z = 0; z < Section.BlockCount; z++)
        {
            for (int x = 0; x < Section.BlockCount; x++)
            {
                // iterate through all sections and set block to top-most of
                // section block if it isn't in exclusions
                foreach (Section section in GetSections())
                {
                    int heightAtSection = section.CoordsAbs.Y * Section.BlockCount;
                    if (heightAtSection > limit)
                        break;

                    for (int y = 0; y < Section.BlockCount; y++)
                    {
                        int height = heightAtSection + Section.BlockCount + y;
                        if (height > limit)
                            break;

                        block = blocks[x, z];

                        // new coords in case if SetBlock successfully modifying the coords
                        coords = new(x + section.CoordsAbs.X * Section.BlockCount,
                                             y + section.CoordsAbs.Y * Section.BlockCount,
                                             z + section.CoordsAbs.Z * Section.BlockCount);

                        // set existing block instance to avoid heap generation.
                        // generating heap at tight-loop like this will trash the GC very badly
                        section.SetBlock(block, coords, relative: false, useAir:false);
                    }
                }
            }
        }
        return blocks;
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
