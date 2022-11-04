namespace binstarjs03.AerialOBJ.Core.NbtNew;

public class NbtString : INbtValue<string>
{
    public string Name { get; set; }
    public string Value { get; set; }

    public NbtString(string name, string value)
    {
        Name = name;
        Value = value;
    }
}
