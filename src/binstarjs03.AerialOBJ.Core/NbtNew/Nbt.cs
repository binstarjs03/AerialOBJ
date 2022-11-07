/*
Nbt Library

Copyright (c) 2022, Bintang Jakasurya
All rights reserved. 

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/



using System.Collections;
using System.Collections.Generic;
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

[DebuggerDisplay("{this.ToString(),nq}")]
public class NbtShort : INbtValue<short>
{
    public string Name { get; set; }
    public NbtType Type => NbtType.NbtShort;
    public short Value { get; set; }

    public NbtShort(string name, short value)
    {
        Name = name;
        Value = value;
    }

    public override string ToString()
    {
        return $"{Type} - \"{Name}\": {Value}";
    }
}

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

[DebuggerDisplay("{this.ToString(),nq}")]
public class NbtFloat : INbtValue<float>
{
    public string Name { get; set; }
    public NbtType Type => NbtType.NbtFloat;
    public float Value { get; set; }

    public NbtFloat(string name, float value)
    {
        Name = name;
        Value = value;
    }

    public override string ToString()
    {
        return $"{Type} - \"{Name}\": {Value}";
    }
}

[DebuggerDisplay("{this.ToString(),nq}")]
public class NbtDouble : INbtValue<double>
{
    public string Name { get; set; }
    public NbtType Type => NbtType.NbtDouble;
    public double Value { get; set; }

    public NbtDouble(string name, double value)
    {
        Name = name;
        Value = value;
    }

    public override string ToString()
    {
        return $"{Type} - \"{Name}\": {Value}";
    }
}

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

[DebuggerDisplay("{this.ToString(),nq}")]
public class NbtCompound : Dictionary<string, INbt>, INbtCollection
{
    public string Name { get; }
    public NbtType Type => NbtType.NbtCompound;

    public NbtCompound(string name)
    {
        Name = name;
    }

    public void Add(INbt nbt)
    {
        Add(nbt.Name, nbt);
    }

    public INbt Get(string nbtName)
    {
        return this[nbtName];
    }

    public T Get<T>(string nbtName) where T : class, INbt
    {
        if (this[nbtName] is not T ret)
            throw new System.InvalidCastException();
        return ret;
    }

    public override string ToString()
    {
        return $"{Type} - \"{Name}\": {Count} nbt(s)";
    }
}

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
