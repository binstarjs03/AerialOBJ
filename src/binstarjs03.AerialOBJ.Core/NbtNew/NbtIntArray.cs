using System.Diagnostics;

namespace binstarjs03.AerialOBJ.Core.NbtNew;

[DebuggerDisplay("{this.ToString(),nq}")]
public class NbtIntArray : INbtArray<int>
{
    public string Name { get; set; }
    public NbtType Type => NbtType.NbtIntArray;
    public int[] Values { get; set; }

    public NbtIntArray(string name, int[] values)
    {
        Name = name;
        Values = values;
    }

    public override string ToString()
    {
        return $"{Type} - \"{Name}\": {Values.Length} value(s)";
    }
}
