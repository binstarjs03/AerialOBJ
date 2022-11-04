namespace binstarjs03.AerialOBJ.Core.NbtNew;

public class NbtShort : INbtValue<short>
{
    public string Name { get; set; }
    public static NbtType Type => NbtType.NbtShort;
    public short Value { get; set; }

    public NbtShort(string name, short value)
    {
        Name = name;
        Value = value;
    }

    public override string ToString()
    {
        return $"<{Type}> {Name} - Value: {Value}";
    }
}
