using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace binstarjs03.AerialOBJ.Core.NbtNew;

[DebuggerDisplay("{this.ToString(),nq}")]
public class NbtList<T> : List<T>, INbtList where T : class, INbt
{
    public string Name { get; }
    public NbtType Type => NbtType.NbtList;
    public NbtType ListType => typeof(T).Name switch
    {
        nameof(NbtType.NbtByte) => NbtType.NbtByte,
        nameof(NbtType.NbtShort) => NbtType.NbtShort,
        nameof(NbtType.NbtInt) => NbtType.NbtInt,
        nameof(NbtType.NbtLong) => NbtType.NbtLong,
        nameof(NbtType.NbtFloat) => NbtType.NbtFloat,
        nameof(NbtType.NbtDouble) => NbtType.NbtDouble,
        nameof(NbtType.NbtString) => NbtType.NbtString,
        nameof(NbtType.NbtByteArray) => NbtType.NbtByteArray,
        nameof(NbtType.NbtIntArray) => NbtType.NbtIntArray,
        nameof(NbtType.NbtLongArray) => NbtType.NbtLongArray,
        nameof(NbtType.NbtCompound) => NbtType.NbtCompound,
        nameof(NbtType.NbtList) => NbtType.NbtList,

        // AerialOBJ specific enumeration
        _ => NbtType.InvalidOrUnknown
    };

    public NbtList(string name)
    {
        Name = name;
    }

    public override string ToString()
    {
        return $"{Type}<{(ListType == NbtType.InvalidOrUnknown ? "Unknown or Any Nbt Type" : ListType)}> - \"{Name}\": {Count} nbt(s)";
    }

    IEnumerator INbtList.GetEnumerator()
    {
        return GetEnumerator();
    }
}
