using System.Collections;
using System.Collections.Generic;

using binstarjs03.AerialOBJ.Core.CoordinateSystem;
using binstarjs03.AerialOBJ.Core.Nbt;

namespace binstarjs03.AerialOBJ.Core.WorldRegion;

public class Block
{
    public const string AirBlockName = "minecraft:air";
    private string _name;
    private Coords3 _coordsAbs;
    private Dictionary<string, string>? _properties;

    public Block()
    {
        _name = AirBlockName;
        _coordsAbs = Coords3.Zero;
    }

    // propertiesless and nameless constructor
    public Block(Coords3 coordsAbs)
    {
        _name = AirBlockName;
        _coordsAbs = coordsAbs;
    }

    // propertiesless constructor
    public Block(string name, Coords3 coordsAbs)
    {
        _name = name;
        _coordsAbs = coordsAbs;
    }

    // TODO properties parser isn't implemented yet. Any properties from nbt
    // compound will be ignored and not stored inside properties dictionary
    public Block(Coords3 coordsAbs, NbtCompound properties)
    {
        _name = properties.Get<NbtString>("Name").Value;
        _coordsAbs = coordsAbs;
    }

    // private constructor for clone method
    private Block(string name, Coords3 coordsAbs, Dictionary<string, string>? properties)
    {
        _name = name;
        _coordsAbs = coordsAbs;
        if (properties is not null)
            _properties = new Dictionary<string, string>(properties);
    }

    public string Name
    {
        get => _name;
        set => _name = value;
    }

    public Coords3 CoordsAbs
    {
        get => _coordsAbs;
        set => _coordsAbs = value;
    }

    public Dictionary<string, string>? Properties
    {
        get => _properties;
        set => _properties = value;
    }

    public static bool IsAir(Block block)
    {
        return block.Name == AirBlockName;
    }

    public static bool IsAir(string blockName)
    {
        return blockName == AirBlockName;
    }

    // TODO Cloning will NOT clone everything. Since properties are reference type,
    // change from original block to other cloned block will affect each other
    public Block Clone()
    {
        return new Block(_name, _coordsAbs, _properties);
    }

    public override string ToString()
    {
        return $"{_name} at {_coordsAbs}";
    }
}
