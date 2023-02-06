namespace binstarjs03.AerialOBJ.Core.MinecraftWorld;
public class Block
{
    public required string Name { get; set; }

    public override string ToString()
    {
        return $"Block {Name}";
    }
}
