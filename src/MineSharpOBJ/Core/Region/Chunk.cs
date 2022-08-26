using System;
using System.Collections.Generic;
using System.Linq;
using binstarjs03.MineSharpOBJ.Core.Utils;
using binstarjs03.MineSharpOBJ.Core.Nbt.Concrete;
namespace binstarjs03.MineSharpOBJ.Core.Region;

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

    public Chunk(NbtCompound nbtChunk, Coords2 coordsRel) {
        _coordsRel = coordsRel;
        _coordsAbs = evaluateCoordsAbs(nbtChunk);
        _blockRangeAbs = evaluateBlockRangeAbs(_coordsAbs);
        _nbtChunk = nbtChunk;
        _nbtSections = initNbtSections(nbtChunk);

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
        static Dictionary<int, NbtCompound> initNbtSections(NbtCompound nbtChunk) {
            Dictionary<int, NbtCompound> ret = new();
            NbtList nbtSections = nbtChunk.Get<NbtList>("sections");
            foreach (NbtCompound nbtSection in nbtSections.Tags.Cast<NbtCompound>()) {
                ret.Add(nbtSection.Get<NbtByte>("Y").Value, nbtSection);
            }
            return ret;
        }
    }

    public Coords2 CoordsRel => _coordsRel;

    public Coords2 CoordsAbs => _coordsAbs;

    public Coords3Range BlockRangeAbs => _blockRangeAbs;

    public NbtCompound NbtChunk => _nbtChunk;

    public Section GetSection(int yPos) {
        if (!_nbtSections.ContainsKey(yPos))
            throw new KeyNotFoundException($"Section {yPos} does not exist in chunk");
        return new Section(this, _nbtSections[yPos]);
    }

    public Block GetBlock(Coords3 coords, bool relative) {
        int sectionYPos = (int)MathF.Floor(coords.y / Section.BlockCount);
        if (relative)
            coords.y = MathUtils.Mod(coords.y, Section.BlockCount);
        //    BlockRangeRel.IsOutside(coords);
        //else {
        //    BlockRangeAbs.IsOutside(coords);
        //    coords = ConvertBlockAbsToRel(coords);
        //}
        Section section = GetSection(sectionYPos);
        return section.GetBlock(coords, relative);
    }

    public static Coords3 ConvertBlockAbsToRel(Coords3 coords) {
        int relBx = MathUtils.Mod(coords.x, Section.BlockCount);
        int relBy = coords.y;
        int relBz = MathUtils.Mod(coords.z, Section.BlockCount);
        return new Coords3(relBx, relBy, relBz);
    }
}
