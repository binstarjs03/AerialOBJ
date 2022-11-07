using System.Diagnostics;

namespace binstarjs03.AerialOBJ.Core.NbtNew;

[DebuggerDisplay("{this.ToString(),nq}")]
public class NbtByteArray : INbtArray<sbyte>
{
    public string Name { get; set; }
    public NbtType Type => NbtType.NbtByteArray;
    public sbyte[] Values { get; set; }

    public NbtByteArray(string name, sbyte[] values)
    {
        Name = name;
        Values = values;
    }

    public override string ToString()
    {
        return $"{Type} - \"{Name}\": {Values.Length} value(s)";
    }
}
