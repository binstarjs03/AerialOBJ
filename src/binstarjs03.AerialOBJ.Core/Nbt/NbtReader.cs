using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using binstarjs03.AerialOBJ.Core.IO;

namespace binstarjs03.AerialOBJ.Core.Nbt;

public class NbtReader : BinaryReaderEndian
{
    private readonly Stack<NbtTypeNamePair> _parsedNbts = new();

    public NbtReader(Stream rawStream) : base(NbtCompression.DecompressStream(rawStream)) { }

    public INbt Parse()
    {
        ReadNbtType(out NbtType type);
        return ReadNbtSwitch(type, insideList: false);
    }

    private INbt ReadNbtSwitch(NbtType type, bool insideList)
    {
        string name = insideList ? "" : ReadStringLengthPrefixed(ByteOrder.BigEndian);
        _parsedNbts.Push(new NbtTypeNamePair(type, name));

        INbt nbt = type switch
        {
            NbtType.NbtCompound => ReadNbtCompound(name),
            NbtType.NbtList => ReadNbtListSwitch(name),
            NbtType.NbtByte => ReadNbtByte(name),
            NbtType.NbtShort => ReadNbtShort(name),
            NbtType.NbtInt => ReadNbtInt(name),
            NbtType.NbtLong => ReadNbtLong(name),
            NbtType.NbtFloat => ReadNbtFloat(name),
            NbtType.NbtDouble => ReadNbtDouble(name),
            NbtType.NbtString => ReadNbtString(name),
            NbtType.NbtByteArray => ReadNbtByteArray(name),
            NbtType.NbtIntArray => ReadNbtIntArray(name),
            NbtType.NbtLongArray => ReadNbtLongArray(name),
            NbtType.NbtEnd => throw new NbtIllegalOperationException($"Cannot instantiate {NbtType.NbtEnd}"),
            _ => throw new NotImplementedException()
        };

        _parsedNbts.Pop();
        return nbt;
    }

    private NbtType ReadNbtType(out NbtType nbtType)
    {
        int type = ReadByte();
        if (type >= 0 && type <= 12)
        {
            nbtType = (NbtType)type;
            return nbtType;
        }
        else
            throw new NbtIllegalTypeException(
                $"Illegal nbt type '{type}' at stream position {BaseStream.Position}");
    }

    private NbtByte ReadNbtByte(string name)
    {
        sbyte value = ReadSByte();
        return new NbtByte(name, value);
    }

    private NbtShort ReadNbtShort(string name)
    {
        short value = ReadShort(ByteOrder.BigEndian);
        return new NbtShort(name, value);
    }

    private NbtInt ReadNbtInt(string name)
    {
        int value = ReadInt(ByteOrder.BigEndian);
        return new NbtInt(name, value);
    }

    private NbtLong ReadNbtLong(string name)
    {
        long value = ReadLong(ByteOrder.BigEndian);
        return new NbtLong(name, value);
    }

    private NbtFloat ReadNbtFloat(string name)
    {
        float value = ReadFloat(ByteOrder.BigEndian);
        return new NbtFloat(name, value);
    }

    private NbtDouble ReadNbtDouble(string name)
    {
        double value = ReadDouble(ByteOrder.BigEndian);
        return new NbtDouble(name, value);
    }

    private NbtString ReadNbtString(string name)
    {
        string value = ReadStringLengthPrefixed(ByteOrder.BigEndian);
        return new NbtString(name, value);
    }

    private NbtByteArray ReadNbtByteArray(string name)
    {
        int arrayLength = ReadInt(ByteOrder.BigEndian);
        sbyte[] array = new sbyte[arrayLength];
        for (int i = 0; i < array.Length; i++)
            array[i] = ReadSByte();
        return new NbtByteArray(name, array);
    }

    private NbtIntArray ReadNbtIntArray(string name)
    {
        int arrayLength = ReadInt(ByteOrder.BigEndian);
        int[] array = new int[arrayLength];
        for (int i = 0; i < array.Length; i++)
            array[i] = ReadInt(ByteOrder.BigEndian);
        return new NbtIntArray(name, array);
    }

    private NbtLongArray ReadNbtLongArray(string name)
    {
        int arrayLength = ReadInt(ByteOrder.BigEndian);
        long[] array = new long[arrayLength];
        for (int i = 0; i < array.Length; i++)
            array[i] = ReadLong(ByteOrder.BigEndian);
        return new NbtLongArray(name, array);
    }

    private NbtCompound ReadNbtCompound(string name)
    {
        NbtCompound nbtCompound = new(name);
        while (ReadNbtType(out NbtType type) != NbtType.NbtEnd)
        {
            INbt nbt = ReadNbtSwitch(type, insideList: false);
            nbtCompound.Add(nbt.Name, nbt);
        }
        return nbtCompound;
    }

    private INbt ReadNbtListSwitch(string name)
    {
        ReadNbtType(out NbtType type);
        return type switch
        {
            /* NbtEnd does not exist so we substitute it with NbtByte,
             * and of course after instantiated, you cannot change
             * the list type whatsoever since it is strongly typed...
             * or we could just use INbt as the (type argument of the) list type,
             * which will accept any kinds of nbt when added
             */
            NbtType.NbtCompound => ReadNbtList<NbtCompound>(name, type),
            NbtType.NbtList => ReadNestedNbtList(name),
            NbtType.NbtEnd => ReadNbtList<NbtByte>(name, type),
            NbtType.NbtByte => ReadNbtList<NbtByte>(name, type),
            NbtType.NbtShort => ReadNbtList<NbtShort>(name, type),
            NbtType.NbtInt => ReadNbtList<NbtInt>(name, type),
            NbtType.NbtLong => ReadNbtList<NbtLong>(name, type),
            NbtType.NbtFloat => ReadNbtList<NbtFloat>(name, type),
            NbtType.NbtDouble => ReadNbtList<NbtDouble>(name, type),
            NbtType.NbtString => ReadNbtList<NbtString>(name, type),
            NbtType.NbtByteArray => ReadNbtList<NbtByteArray>(name, type),
            NbtType.NbtIntArray => ReadNbtList<NbtIntArray>(name, type),
            NbtType.NbtLongArray => ReadNbtList<NbtLongArray>(name, type),
            _ => throw new NbtIllegalTypeException()
        };
    }

    private INbtList ReadNestedNbtList(string name)
    {
        /* Reading nested NbtList is tricky because we can't determine what the
         * type of the NbtList is (can't instantiate, undefined type argument)
         * until we actually read what items that instance of NbtList is holding.
         *
         * To solve this, we have to polymorph the type argument to its top-level inheritance, 
         * which is the interface of INbt in this case, and the return type as the nearest
         * non-generic inheritance, which is INbtList.
         * 
         * Technically we can inline the call in ReadNbtListSwitch at switch NbtType.NbtList
         * with => ReadNbtList<INbt>, but else we cannot make a documentation comment like this :)
         */
        return ReadNbtList<INbt>(name, NbtType.NbtList);
    }

    private NbtList<T> ReadNbtList<T>(string name, NbtType listType) where T : class, INbt
    {
        int listLength = ReadInt(ByteOrder.BigEndian);
        NbtList<T> nbtList = new(name, listLength);
        for (int i = 0; i < listLength; i++)
        {
            T nbt = (ReadNbtSwitch(listType, insideList: true) as T)!;
            nbtList.Add(nbt);
        }
        return nbtList;
    }

    public string GetParseErrorAsString()
    {
        StringBuilder sb = new();
        NbtTypeNamePair errorNbt = _parsedNbts.Pop();
        IEnumerable<NbtTypeNamePair> reversedNbtStack = _parsedNbts.Reverse();

        sb.AppendLine("Nbt Stack: ");
        foreach (NbtTypeNamePair nbt in reversedNbtStack)
        {
            sb.Append("    ");
            sb.Append(nbt.Type);
            sb.Append(" - ");
            sb.AppendLine(nbt.Name);
        }
        sb.Append($"An error occured while parsing nbt data of {errorNbt.Type} - {errorNbt.Name}");
        _parsedNbts.Push(errorNbt);
        return sb.ToString();
    }
}
