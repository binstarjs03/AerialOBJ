namespace binstarjs03.AerialOBJ.Core.NbtNew;

public class NbtString : INbtValue<string>
{
    public string Name { get; set; }
    public static NbtType Type => NbtType.NbtString;
    public string Value { get; set; }

    public NbtString(string name, string value)
    {
        Name = name;
        Value = value;
    }

    public override string ToString()
    {
        return $"<{Type}> {Name} - Value: {Value}";
    }
}
