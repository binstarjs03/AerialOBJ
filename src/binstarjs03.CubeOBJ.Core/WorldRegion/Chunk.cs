using System;
using System.Collections.Generic;
using System.Linq;

using binstarjs03.AerialOBJ.Core.CoordinateSystem;
using binstarjs03.AerialOBJ.Core.Nbt;

namespace binstarjs03.AerialOBJ.Core.WorldRegion;

public class Chunk
{
    public static readonly int TotalSectionCount = SectionRange.max - SectionRange.min;
    public static readonly (int min, int max) SectionRange = (-4, 19);
    public static readonly int TotalBlockCount = Section.TotalBlockCount * TotalSectionCount;
    public static readonly CoordsRange3 BlockRangeRel = new(
        xRange: new Range(0, Section.BlockCount),
        yRange: new Range(Section.BlockCount * SectionRange.min, Section.BlockCount * SectionRange.max + Section.BlockRange),
        zRange: new Range(0, Section.BlockCount)
    );

    private readonly Coords2 _coordsRel;
    private readonly Coords2 _coordsAbs;
    private readonly CoordsRange3 _blockRangeAbs;
    private readonly NbtCompound _nbtChunk;
    private readonly Dictionary<int, NbtCompound> _nbtSections;
    private readonly int[] _sectionsYPos;

    public Chunk(NbtCompound nbtChunk, Coords2 coordsRel)
    {
        _coordsRel = coordsRel;
        _coordsAbs = evaluateCoordsAbs(nbtChunk);
        _blockRangeAbs = evaluateBlockRangeAbs(_coordsAbs);
        _nbtChunk = nbtChunk;
        (_sectionsYPos, _nbtSections) = initSections(nbtChunk);

        static Coords2 evaluateCoordsAbs(NbtCompound nbtChunk)
        {
            int x = nbtChunk.Get<NbtInt>("xPos").Value;
            int z = nbtChunk.Get<NbtInt>("zPos").Value;
            return new Coords2(x, z);
        }
        static CoordsRange3 evaluateBlockRangeAbs(Coords2 coordsAbs)
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
        static (int[], Dictionary<int, NbtCompound>) initSections(NbtCompound nbtChunk)
        {
            NbtList nbtSections = nbtChunk.Get<NbtList>("sections");
            int sectionLength = nbtSections.Length;

            int[] retInts = new int[nbtSections.Length];
            Dictionary<int, NbtCompound> retDict = new();

            for (int i = 0; i < sectionLength; i++)
            {
                NbtCompound nbtSection = nbtSections.Get<NbtCompound>(i);
                int sectionYPos = nbtSection.Get<NbtByte>("Y").Value;

                if (!nbtSection.HasTag("block_states"))
                    // TODO: send warning signal that this chunk contains skipped section
                    continue;

                retInts[i] = sectionYPos;
                retDict.Add(sectionYPos, nbtSection);
            }
            return (retInts, retDict);
        }
    }

    public Coords2 CoordsRel => _coordsRel;

    public Coords2 CoordsAbs => _coordsAbs;

    public CoordsRange3 BlockRangeAbs => _blockRangeAbs;

    public NbtCompound NbtChunk => _nbtChunk;

    public int[] SectionsYPos => _sectionsYPos;

    public Section GetSection(int yPos)
    {
        if (!_nbtSections.ContainsKey(yPos))
            throw new KeyNotFoundException($"Section {yPos} does not exist in chunk");
        return new Section(this, _nbtSections[yPos]);
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
        int sectionYPos = (int)MathF.Floor(coords.Y / Section.BlockCount);
        if (relative)
            coords.Y = MathUtils.Mod(coords.Y, Section.BlockCount);
        Section section = GetSection(sectionYPos);
        return section.GetBlock(coords, relative);
    }

    public Block[] GetBlockTopmost(string[] exclusions)
    {
        int sectionBlockCountXZ = (int)MathF.Pow(Section.BlockCount, 2);
        Block[] topBlocks = new Block[sectionBlockCountXZ];
        foreach (Section section in GetSections())
        {
            for (int z = 0; z < Section.BlockCount; z++)
            {
                for (int x = 0; x < Section.BlockCount; x++)
                {
                    Block? topBlockAtThisXZ = null;
                    for (int y = 0; y < Section.BlockCount; y++)
                    {
                        Coords3 coordsBlockRel = new(x, y, z);
                        Block block = section.GetBlock(coordsBlockRel, relative: true);
                        //if (topmost[index].Name == block.Name)
                        //    continue;
                        if (exclusions.Contains(block.Name))
                            continue;
                        topBlockAtThisXZ = block;
                    }
                    int index = x + z * Section.BlockCount;
                    if (topBlockAtThisXZ is not null)
                        topBlocks[index] = topBlockAtThisXZ;
                }
            }
        }
        return topBlocks;
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
