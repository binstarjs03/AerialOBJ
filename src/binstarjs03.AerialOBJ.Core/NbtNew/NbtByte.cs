namespace binstarjs03.AerialOBJ.Core.NbtNew;

public class NbtByte : INbtValue<byte>
{
    public string Name { get; set; }
    public static NbtType Type => NbtType.NbtByte;
    public byte Value { get; set; }

    public NbtByte(string name, byte value)
    {
        Name = name;
        Value = value;
    }

    public override string ToString()
    {
        return $"<{Type}> {Name} - Value: {Value}";
    }
}

