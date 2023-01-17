namespace binstarjs03.AerialOBJ.Core.MinecraftWorld;
/// <summary>
/// Lightweight version of <see cref="Block"/> that only store 
/// the name and the height. Used for high-performance situation
/// </summary>
public struct BlockSlim
{
    public BlockSlim(string name, int height)
    {
        Name = name;
        Height = height;
    }

    public string Name { get; set; }
    public int Height { get; set; }

    public override string ToString()
    {
        return $"Block {Name} - Height: {Height}";
    }
}
