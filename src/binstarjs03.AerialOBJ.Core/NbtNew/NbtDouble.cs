namespace binstarjs03.AerialOBJ.Core.NbtNew;

public class NbtDouble : INbtValue<double>
{
    public string Name { get; set; }
    public static NbtType Type => NbtType.NbtDouble;
    public double Value { get; set; }

    public NbtDouble(string name, double value)
    {
        Name = name;
        Value = value;
    }

    public override string ToString()
    {
        return $"<{Type}> {Name} - Value: {Value}";
    }
}
