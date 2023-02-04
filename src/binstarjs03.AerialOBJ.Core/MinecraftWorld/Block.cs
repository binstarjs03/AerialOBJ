using binstarjs03.AerialOBJ.Core.Primitives;

namespace binstarjs03.AerialOBJ.Core.MinecraftWorld;
public struct Block
{
    public const string AirBlockName = "minecraft:air";
    public const string CaveAirBlockName = "minecraft:cave_air";

    private static readonly Block s_air = new()
    {
        Name = AirBlockName,
        Coords = Point3<int>.Zero,
    };

    public Block() { }

    public static ref readonly Block Air => ref s_air;

    public required string Name { get; set; } = AirBlockName;
    public required Point3<int> Coords { get; set; } = Point3<int>.Zero;

    // TODO maybe we should use definitions and check if Name is in list
    // of air blocks instead of hardcoding it to some string literal
    public bool IsAir => Name == AirBlockName
                      || Name == CaveAirBlockName;

    public override string ToString()
    {
        return $"Block {Name} - {Coords}";
    }
}
