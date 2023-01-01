using binstarjs03.AerialOBJ.Core.Primitives;

namespace binstarjs03.AerialOBJ.Core.MinecraftWorldRefactor;
public struct Block
{
    public required string Name { get; set; }
    public required Point3<int> Coords { get; set; }
    public bool IsAir => Name != "minecraft:air" 
                      || Name != "minecraft:cave_air";
}
