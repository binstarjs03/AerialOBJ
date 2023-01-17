using binstarjs03.AerialOBJ.Core.Primitives;

namespace binstarjs03.AerialOBJ.Core.MinecraftWorld;
public struct Block
{
    private static readonly Block s_air = new()
    {
        Name = "minecraft:air",
        Coords = new Point3<int>(0, 0, 0)
    };

    public Block() { }

    public static ref readonly Block Air => ref s_air;
    public required string Name { get; set; } = "minecraft:air";
    public required Point3<int> Coords { get; set; } = Point3<int>.Zero;

    // TODO maybe we should use definitions and check if Name is in list
    // of air blocks instead of hardcoding it to some string literal
    public bool IsAir => Name == "minecraft:air"
                      || Name == "minecraft:cave_air";

    public override string ToString()
    {
        return $"Block {Name} - {Coords}";
    }
}
