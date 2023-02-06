namespace binstarjs03.AerialOBJ.Core.MinecraftWorld;

/// <summary>
/// Lightweight version of <see cref="Block"/> that only store 
/// the name and the height. XZ Coordinate can be inferred from array 
/// index if it is an array element of chunk block buffer. 
/// Used when high performance, Zero-GC allocation required
/// </summary>
public struct BlockSlim
{
    public BlockSlim(string name, int height)
    {
        Name = name;
        Height = height;
    }

    /// <summary>
    /// Name of this block, including the namespace
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Absolute Y position of this block
    /// </summary>
    public int Height { get; set; }

    public override string ToString()
    {
        return $"Block {Name} - Height: {Height}";
    }
}
