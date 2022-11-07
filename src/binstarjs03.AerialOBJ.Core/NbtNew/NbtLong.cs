using System.Diagnostics;

namespace binstarjs03.AerialOBJ.Core.NbtNew;

[DebuggerDisplay("{this.ToString(),nq}")]
public class NbtLong : INbtValue<long>
{
    public string Name { get; set; }
    public NbtType Type => NbtType.NbtLong;
    public long Value { get; set; }

    public NbtLong(string name, long value)
    {
        Name = name;
        Value = value;
    }

    public override string ToString()
    {
        return $"{Type} - \"{Name}\": {Value}";
    }
}
