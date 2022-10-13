using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using binstarjs03.AerialOBJ.Core.IO;

namespace binstarjs03.AerialOBJ.Core.Nbt.IO;

public class NbtBinaryReader : BinaryReaderEndian
{
    public readonly Stack<NbtBase> NbtTagStack = new();

    public NbtBinaryReader(Stream input, ByteOrder byteOrder) : base(input, byteOrder) { }

    /// <exception cref="EndOfStreamException"></exception>
    /// <exception cref="ObjectDisposedException"></exception>
    /// <exception cref="IOException"></exception>
    /// <exception cref="NbtUnknownTagTypeException"></exception>
    public NbtType ReadTagType()
    {
        int type = ReadByte();
        if (Enum.IsDefined(typeof(NbtType), type))
            return (NbtType)type;
        else
            throw new NbtUnknownTagTypeException(
                $"Unknown tag type '{type}' at stream position {BaseStream.Position}"
            );
    }

    public string GetReadingErrorStackAsString()
    {
        StringBuilder sb = new();
        NbtBase errorNbt = NbtTagStack.Pop();
        IEnumerable<NbtBase> reversedNbtStack = NbtTagStack.Reverse();

        sb.AppendLine("Nbt tag stack: ");
        foreach (NbtBase nbt in reversedNbtStack)
        {
            sb.Append("    ");
            sb.Append(nbt.NbtType);
            sb.Append(" - ");
            sb.AppendLine(nbt.Name);
        }
        sb.Append($"An error occured while parsing nbt data of {errorNbt.NbtType} - {errorNbt.Name}");
        NbtTagStack.Push(errorNbt);
        return sb.ToString();
    }
}
