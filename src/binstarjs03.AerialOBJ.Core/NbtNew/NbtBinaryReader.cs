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
