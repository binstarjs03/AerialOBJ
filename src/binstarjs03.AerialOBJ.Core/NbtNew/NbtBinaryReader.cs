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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace binstarjs03.AerialOBJ.Core.NbtNew;

public class NbtBinaryReader : BinaryReaderEndian
{
    private readonly Stack<(NbtType, string)> _stack = new();

    // Push as soon an nbt tag type and its name are successfully parsed,
    // then pop when a tag is completely parsed
    public Stack<(NbtType, string)> Stack => _stack;

    public NbtBinaryReader(Stream input) : base(input) { }

    /// <exception cref="NbtIllegalTypeException"></exception>
    public NbtType ReadTagType()
    {
        int type = ReadByte();
        if (Enum.IsDefined(typeof(NbtType), type))
            return (NbtType)type;
        else
            throw new NbtIllegalTypeException(
                $"Unknown tag type '{type}' at stream position {_stream.Position}"
            );
    }

    public string GetNbtStackParseErrorAsString()
    {
        StringBuilder sb = new();
        (NbtType type, string name) errorNbt = _stack.Pop();
        IEnumerable<(NbtType, string)> reversedNbtStack = _stack.Reverse();

        sb.AppendLine("Nbt Stack: ");
        foreach ((NbtType type, string name) nbt in reversedNbtStack)
        {
            sb.Append("    ");
            sb.Append(nbt.type);
            sb.Append(" - ");
            sb.AppendLine(nbt.name);
        }
        sb.Append($"An error occured while parsing nbt data of {errorNbt.type} - {errorNbt.name}");
        _stack.Push(errorNbt);
        return sb.ToString();
    }
}
