using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using binstarjs03.MinecraftSharpOBJ.Nbt.Abstract;
using binstarjs03.MinecraftSharpOBJ.Utils.IO;
using BinaryReader = binstarjs03.MinecraftSharpOBJ.Utils.IO.BinaryReader;
namespace binstarjs03.MinecraftSharpOBJ.Nbt.IO;

public class NbtBinaryReader : BinaryReader {
    public readonly Stack<NbtBase> NbtStackReadingState = new();

    public NbtBinaryReader(Stream input, ByteOrder byteOrder) : base(input, byteOrder) {
        return;
    }

    /// <exception cref="NbtUnknownTagTypeException"></exception>
    public NbtType ReadTagType() {
        int type = ReadByte();
        if (Enum.IsDefined(typeof(NbtType), type))
            return (NbtType)type;
        else
            throw new NbtUnknownTagTypeException(
            $"Unknown tag type at stream position {BaseStream.Position}"
        );
    }

    public string GetReadingErrorStack() {
        StringBuilder sb = new();
        NbtBase errorNbt = NbtStackReadingState.Pop();
        IEnumerable<NbtBase> reversedNbtStack = NbtStackReadingState.Reverse();

        sb.AppendLine("Nbt tag stack: ");
        foreach (NbtBase nbt in reversedNbtStack) {
            sb.Append("    ");
            sb.Append(nbt.NbtTypeName);
            sb.Append(" - ");
            sb.AppendLine(nbt.Name);
        }
        sb.Append($"An error occured while parsing nbt data of {errorNbt.NbtType} - {errorNbt.Name}");
        return sb.ToString();
    }
}
