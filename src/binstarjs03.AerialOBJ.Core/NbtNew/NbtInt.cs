namespace binstarjs03.AerialOBJ.Core.NbtNew;

public class NbtInt : INbtValue<int>
{
    public string Name { get; set; }
    public int Value { get; set; }
    public NbtInt(string name, int value)
    {
        Name = name;
        Value = value;
    }
}
