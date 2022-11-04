namespace binstarjs03.AerialOBJ.Core.NbtNew;

public class NbtByte : INbtValue<byte>
{
    public string Name { get; set; }
    public byte Value { get; set; }

    public NbtByte(string name, byte value)
    {
        Name = name;
        Value = value;
    }
}

