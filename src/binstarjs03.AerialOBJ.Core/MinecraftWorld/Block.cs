using System.Collections.Generic;

using binstarjs03.AerialOBJ.Core.CoordinateSystem;
using binstarjs03.AerialOBJ.Core.NbtNew;

namespace binstarjs03.AerialOBJ.Core.MinecraftWorld;

public class Block
{
    public const string AirBlockName = "minecraft:air";
    private string _name;
    private Coords3 _blockCoordsAbs;
    private Dictionary<string, string>? _properties;

    public static Block Air => new();

    public string Name
    {
        get => _name;
        set => _name = value;
    }

    public Coords3 BlockCoordsAbs
    {
        get => _blockCoordsAbs;
        set => _blockCoordsAbs = value;
    }

    public Dictionary<string, string>? Properties
    {
        get => _properties;
        set => _properties = value;
    }

    public Block()
    {
        _name = AirBlockName;
        _blockCoordsAbs = Coords3.Zero;
    }

    // propertiesless and nameless constructor
    public Block(Coords3 coordsAbs)
    {
        _name = AirBlockName;
        _blockCoordsAbs = coordsAbs;
    }

    // propertiesless constructor
    public Block(string name, Coords3 coordsAbs)
    {
        _name = name;
        _blockCoordsAbs = coordsAbs;
    }

    // TODO properties parser isn't implemented yet. Any properties from nbt
    // compound will be ignored and not stored inside properties dictionary
    public Block(Coords3 coordsAbs, NbtCompound properties)
    {
        _name = properties.Get<NbtString>("Name").Value;
        _blockCoordsAbs = coordsAbs;
    }

    // private constructor for clone method
    private Block(string name, Coords3 coordsAbs, Dictionary<string, string>? properties)
    {
        _name = name;
        _blockCoordsAbs = coordsAbs;
        if (properties is not null)
            _properties = new Dictionary<string, string>(properties);
    }

    public static bool IsAir(Block block)
    {
        return block.Name == AirBlockName;
    }

    public static bool IsAir(string blockName)
    {
        return blockName == AirBlockName;
    }

    public override string ToString()
    {
        return $"Block {_name} at {_blockCoordsAbs}";
    }
}
