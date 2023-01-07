using binstarjs03.AerialOBJ.Core.Primitives;

namespace binstarjs03.AerialOBJ.Core.MinecraftWorld;
public struct Block
{
    public Block() { }

    public required string Name { get; set; } = "minecraft:air";
    public required Point3<int> Coords { get; set; } = Point3<int>.Zero;
    public bool IsAir => Name == "minecraft:air"
                      || Name == "minecraft:cave_air";
}
