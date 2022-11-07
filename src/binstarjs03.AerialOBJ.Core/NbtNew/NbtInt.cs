using System.Diagnostics;

namespace binstarjs03.AerialOBJ.Core.NbtNew;

[DebuggerDisplay("{this.ToString(),nq}")]
public class NbtInt : INbtValue<int>
{
    public string Name { get; set; }
    public NbtType Type => NbtType.NbtInt;
    public int Value { get; set; }

    public NbtInt(string name, int value)
    {
        Name = name;
        Value = value;
    }

    public override string ToString()
    {
        return $"{Type} - \"{Name}\": {Value}";
    }
}
