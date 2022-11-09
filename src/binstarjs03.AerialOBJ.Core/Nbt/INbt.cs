/*
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



// These are interfaces for linking different types of nbt as the implementation
// of AerialOBJ nbt does not have common single base class inheritance.
// I decided to use interface because i want NbtList and NbtCompound to derive from
// List<T> and Dictionary<T> respectively, so to glue all different nbt types together,
// we used interfaces



using System.Collections;

namespace binstarjs03.AerialOBJ.Core.Nbt;

public interface INbt
{
    public string Name { get; }
    public NbtType Type { get; }
}

public interface INbtArray<T> : INbt
{
    public T[] Values { get; }
}

public interface INbtCollection : INbt { }

/// <summary>
/// Ultimate Polymorphism for undetermined argument type of <see cref="NbtList{T}"/>
/// </summary>
public interface INbtList : INbtCollection
{
    public NbtType ListType { get; }
    public int Count { get; }
    public IEnumerator GetEnumerator();
}

public interface INbtValue<T> : INbt
{
    public T Value { get; }
}
