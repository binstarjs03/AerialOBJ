using binstarjs03.CubeOBJ.Core.CoordinateSystem;
using binstarjs03.CubeOBJ.Core.Nbt;

namespace binstarjs03.CubeOBJ.Core.WorldRegion;

public class Block
{
    private readonly string _name;
    private readonly Coords3 _coordsRel;
    private readonly Coords3 _coordsAbs;
    private readonly NbtCompound _properties;

    public Block(Section section, Coords3 coordsRel)
    {
        _coordsRel = coordsRel;
        _coordsAbs = EvaluateCoordsAbs(section, coordsRel);
        string name = "minecraft:air";
        NbtBase[] propertiesNbt = new NbtBase[1] { new NbtString("Name", name) };
        _name = name;
        _properties = new(propertiesNbt);
    }

    public Block(Section section, Coords3 coordsRel, NbtCompound properties)
    {
        _coordsRel = coordsRel;
        _coordsAbs = EvaluateCoordsAbs(section, coordsRel);
        _name = properties.Get<NbtString>("Name").Value;
        _properties = properties;
    }

    private static Coords3 EvaluateCoordsAbs(Section section, Coords3 coordsRel)
    {
        int x = coordsRel.X + (section.CoordsAbs.X * Section.BlockCount);
        int y = coordsRel.Y + (section.CoordsAbs.Y * Section.BlockCount);
        int z = coordsRel.Z + (section.CoordsAbs.Z * Section.BlockCount);
        return new Coords3(x, y, z);
    }

    public string Name => _name;

    public Coords3 CoordsRel => _coordsRel;

    public Coords3 CoordsAbs => _coordsAbs;

    public NbtCompound Properties => _properties;

    public override string ToString()
    {
        return $"{_name} block {_coordsAbs}";
    }
}
