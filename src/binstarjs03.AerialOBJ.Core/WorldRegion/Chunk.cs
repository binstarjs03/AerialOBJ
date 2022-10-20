using System;
using System.Collections.Generic;
using System.Linq;

using binstarjs03.AerialOBJ.Core.CoordinateSystem;
using binstarjs03.AerialOBJ.Core.Nbt;

namespace binstarjs03.AerialOBJ.Core.WorldRegion;

public class Chunk
{
    // TODO: these static fields are unreliable. some custom worlds have their own range
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
    private readonly NbtCompound _nbtChunk;
    private readonly Dictionary<int, NbtCompound> _nbtSections;
    private readonly int[] _sectionsYPos;

    public Chunk(NbtCompound nbtChunk)
    {
        _coordsAbs = calculateCoordsAbs(nbtChunk);
        _blockRangeAbs = calculateBlockRangeAbs(_coordsAbs);
        _nbtChunk = nbtChunk;
        (_sectionsYPos, _nbtSections) = readSectionsTag(nbtChunk);

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
        static (int[], Dictionary<int, NbtCompound>) readSectionsTag(NbtCompound nbtChunk)
        {
            NbtList nbtSections = nbtChunk.Get<NbtList>("sections");
            int sectionLength = nbtSections.Length;

            int[] sectionInts = new int[nbtSections.Length];
            Dictionary<int, NbtCompound> sectionDict = new();

            for (int i = 0; i < sectionLength; i++)
            {
                NbtCompound nbtSection = nbtSections.Get<NbtCompound>(i);
                int sectionYPos = nbtSection.Get<NbtByte>("Y").Value;

                sectionInts[i] = sectionYPos;
                sectionDict.Add(sectionYPos, nbtSection);
            }
            return (sectionInts, sectionDict);
        }
    }

    public Coords2 CoordsAbs => _coordsAbs;

    public CoordsRange3 BlockRangeAbs => _blockRangeAbs;

    public NbtCompound NbtChunk => _nbtChunk;

    public int[] SectionsYPos => _sectionsYPos;

    public bool HasSection(int sectionY)
    {
        if (_nbtSections.ContainsKey(sectionY))
            return true;
        return false;
    }

    public Section GetSection(int sectionY)
    {
        if (!_nbtSections.ContainsKey(sectionY))
            throw new KeyNotFoundException($"Section {sectionY} does not exist in chunk");
        return new Section(this, _nbtSections[sectionY]);
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

        foreach (Section section in GetSections())
        {
            int heightAtSection = section.CoordsAbs.Y * Section.BlockCount;
            if (heightAtSection > limit)
                break;
            for (int z = 0; z < Section.BlockCount; z++)
            {
                for (int x = 0; x < Section.BlockCount; x++)
                {
                    for (int y = 0; y < Section.BlockCount; y++)
                    {
                        Coords3 coordsBlockAbs = new(section.CoordsAbs.X * Section.BlockCount + x,
                                                     section.CoordsAbs.Y * Section.BlockCount + y,
                                                     section.CoordsAbs.Z * Section.BlockCount + z);
                        if (blocks[x,z] is null)
                        {
                            Block air = new(coordsBlockAbs);
                            blocks[x, z] = air;
                        }

                        int height = heightAtSection + Section.BlockCount + y;
                        if (height > limit)
                            break;

                        Block block = section.GetBlock(coordsBlockAbs, relative: false);
                        if (exclusions.Contains(block.Name))
                            continue;
                        blocks[x, z] = block;
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
