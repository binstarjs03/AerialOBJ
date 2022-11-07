﻿using System.Diagnostics;

namespace binstarjs03.AerialOBJ.Core.NbtNew;

[DebuggerDisplay("{this.ToString(),nq}")]
public class NbtLongArray : INbtArray<long>
{
    public string Name { get; set; }
    public NbtType Type => NbtType.NbtLongArray;
    public long[] Values { get; set; }

    public NbtLongArray(string name, long[] values)
    {
        Name = name;
        Values = values;
    }

    public override string ToString()
    {
        return $"{Type} - \"{Name}\": {Values.Length} value(s)";
    }
}
