using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using binstarjs03.AerialOBJ.Core.IO;

namespace binstarjs03.AerialOBJ.Core.NbtNew;

public class NbtBinaryReader : BinaryReaderEndian
{
    private readonly Stack<(NbtType, string)> _stack = new();

    // Push as soon an nbt tag is first encountered, then pop when a tag is completely parsed
    public Stack<(NbtType, string)> Stack => _stack;

    public NbtBinaryReader(Stream input) : base(input) { }

    /// <exception cref="NbtUnknownTypeException"></exception>
    public NbtType ReadTagType()
    {
        int type = ReadByte();
        if (Enum.IsDefined(typeof(NbtType), type))
            return (NbtType)type;
        else
            throw new NbtUnknownTypeException(
                $"Unknown tag type '{type}' at stream position {_stream.Position}"
            );
    }

    public string GetReadingErrorStackAsString()
    {
        StringBuilder sb = new();
        (NbtType type, string name) errorNbt = _stack.Pop();
        IEnumerable<(NbtType, string)> reversedNbtStack = _stack.Reverse();

        sb.AppendLine("Nbt tag stack: ");
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
