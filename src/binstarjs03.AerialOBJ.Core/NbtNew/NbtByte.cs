using System.Diagnostics;

namespace binstarjs03.AerialOBJ.Core.NbtNew;

[DebuggerDisplay("{this.ToString(),nq}")]
public class NbtByte : INbtValue<sbyte>
{
    public string Name { get; set; }
    public NbtType Type => NbtType.NbtByte;
    public sbyte Value { get; set; }

    public NbtByte(string name, sbyte value)
    {
        Name = name;
        Value = value;
    }

    public override string ToString()
    {
        return $"{Type} - \"{Name}\": {Value}";
    }
}

