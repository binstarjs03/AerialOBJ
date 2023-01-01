using binstarjs03.AerialOBJ.Core.Primitives;

namespace binstarjs03.AerialOBJ.Core.MinecraftWorld;
public struct BlockStruct
{
    public required string Name { get; set; }
    public required Point3<int> CoordsAbs { get; set; }
}
