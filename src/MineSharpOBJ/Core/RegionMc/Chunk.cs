using System;
using System.Collections.Generic;
using System.Linq;
using binstarjs03.MineSharpOBJ.Core.Utils;
using binstarjs03.MineSharpOBJ.Core.Nbt.Concrete;
namespace binstarjs03.MineSharpOBJ.Core.RegionMc;

public class Chunk {
    public static readonly int TotalSectionCount = SectionRange.max - SectionRange.min;
    public static readonly (int min, int max) SectionRange = (-4, 19);
    public static readonly int TotalBlockCount = Section.TotalBlockCount * TotalSectionCount;
    public static readonly Coords3Range BlockRangeRel = new(
        min: new Coords3(
            x: 0,
            y: Section.BlockCount * SectionRange.min,
            z: 0),
        max: new Coords3(
            x: Section.BlockCount,
            y: Section.BlockCount * SectionRange.max + Section.BlockRange,
            z: Section.BlockCount)
    );

    private readonly Coords2 _coordsRel;
    private readonly Coords2 _coordsAbs;
    private readonly Coords3Range _blockRangeAbs;
    private readonly NbtCompound _nbtChunk;
    private readonly Dictionary<int, NbtCompound> _nbtSections;
    private readonly int[] _sectionsYPos;

    public Chunk(NbtCompound nbtChunk, Coords2 coordsRel) {
        _coordsRel = coordsRel;
        _coordsAbs = evaluateCoordsAbs(nbtChunk);
        _blockRangeAbs = evaluateBlockRangeAbs(_coordsAbs);
        _nbtChunk = nbtChunk;
        (_sectionsYPos, _nbtSections) = initSections(nbtChunk);

        static Coords2 evaluateCoordsAbs(NbtCompound nbtChunk) {
            int x = nbtChunk.Get<NbtInt>("xPos").Value;
            int z = nbtChunk.Get<NbtInt>("zPos").Value;
            return new Coords2(x, z);
        }
        static Coords3Range evaluateBlockRangeAbs(Coords2 coordsAbs) {
            int minAbsBx = coordsAbs.x * Section.BlockCount;
            int minAbsBy = SectionRange.min * Section.BlockCount;
            int minAbsBz = coordsAbs.z * Section.BlockCount;
            Coords3 minAbsB = new(minAbsBx, minAbsBy, minAbsBz);

            int maxAbsBx = minAbsBx + Section.BlockRange;
            int maxAbsBy = SectionRange.max * Section.BlockCount + Section.BlockRange;
            int maxAbsBz = minAbsBz + Section.BlockRange;
            Coords3 maxAbsB = new(maxAbsBx, maxAbsBy, maxAbsBz);

            return new Coords3Range(minAbsB, maxAbsB);
        }
        static (int[], Dictionary<int, NbtCompound>) initSections(NbtCompound nbtChunk) {
            NbtList nbtSections = nbtChunk.Get<NbtList>("sections");
            int sectionLength = nbtSections.Length;

            int[] retInts = new int[nbtSections.Length];
            Dictionary<int, NbtCompound> retDict = new();

            for (int i = 0; i < sectionLength; i++) {
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

    public Coords3Range BlockRangeAbs => _blockRangeAbs;

    public NbtCompound NbtChunk => _nbtChunk;

    public int[] SectionsYPos => _sectionsYPos;

    public Section GetSection(int yPos) {
        if (!_nbtSections.ContainsKey(yPos))
            throw new KeyNotFoundException($"Section {yPos} does not exist in chunk");
        return new Section(this, _nbtSections[yPos]);
    }

    public Block GetBlock(Coords3 coords, bool relative) {
        int sectionYPos = (int)MathF.Floor(coords.y / Section.BlockCount);
        if (relative)
            coords.y = MathUtils.Mod(coords.y, Section.BlockCount);
        Section section = GetSection(sectionYPos);
        return section.GetBlock(coords, relative);
    }

    public static Coords3 ConvertBlockAbsToRel(Coords3 coords) {
        int relBx = MathUtils.Mod(coords.x, Section.BlockCount);
        int relBy = coords.y;
        int relBz = MathUtils.Mod(coords.z, Section.BlockCount);
        return new Coords3(relBx, relBy, relBz);
    }

    public override string ToString() {
        return $"Chunk {_coordsAbs}";
    }
}
