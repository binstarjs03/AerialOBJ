using System.Diagnostics;

namespace binstarjs03.AerialOBJ.Core.NbtNew;

[DebuggerDisplay("{this.ToString(),nq}")]
public class NbtString : INbtValue<string>
{
    public string Name { get; set; }
    public NbtType Type => NbtType.NbtString;
    public string Value { get; set; }

    public NbtString(string name, string value)
    {
        Name = name;
        Value = value;
    }

    public override string ToString()
    {
        return $"{Type} - \"{Name}\": \"{Value}\"";
    }
}
