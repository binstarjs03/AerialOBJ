namespace binstarjs03.AerialOBJ.Core.NbtNew;

public class NbtLong : INbtValue<long>
{
    public string Name { get; set; }
    public long Value { get; set; }

    public NbtLong(string name, long value)
    {
        Name = name;
        Value = value;
    }
}
