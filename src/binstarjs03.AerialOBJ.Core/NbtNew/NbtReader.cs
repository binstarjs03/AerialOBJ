using System;
using System.IO.Compression;
using System.IO;

namespace binstarjs03.AerialOBJ.Core.NbtNew;

public static class NbtReader
{
    private enum ByteHeader
    {
        Gzip = 0x1F,
        Zlib = 0x78,
        TagCompound = 0x0A,
    }

    public static INbt ReadDisk(string path)
    {
        MemoryStream ms = new(File.ReadAllBytes(path));
        return ReadStream(ms);
    }

    /// <exception cref="NbtDeserializationError"></exception>
    public static INbt ReadStream(Stream inputStream)
    {
        Stream stream = DecompressNbtStream(inputStream);
        NbtBinaryReader reader = new(stream);
        NbtType nbtType = ReadNbtType(reader);
        try
        {
            return ReadNbtSwitch(reader, nbtType, insideList: false);
        }
        catch (Exception e)
        {
            throw new NbtDeserializationError(
                $"Exception occured while parsing serialized nbt data.\n" +
                $"Exception details: {e}\n" +
                reader.GetNbtStackParseErrorAsString(), e);
        }
    }

    private static MemoryStream DecompressNbtStream(Stream inputStream)
    {
        MemoryStream decompressedStream = new();
        int byteHeader = inputStream.ReadByte();
        inputStream.Seek(-1, SeekOrigin.Current);
        switch (byteHeader)
        {
            case (int)ByteHeader.Gzip:
                using (GZipStream gZipStream = new(inputStream, CompressionMode.Decompress))
                    gZipStream.CopyTo(decompressedStream);
                break;
            case (int)ByteHeader.Zlib:
                using (ZLibStream zLibStream = new(inputStream, CompressionMode.Decompress))
                    zLibStream.CopyTo(decompressedStream);
                break;
            case (int)ByteHeader.TagCompound:
                inputStream.CopyTo(decompressedStream);
                break;
            default:
                throw new NbtUnknownCompressionMethodException(
                $"Undefined compression byte header of {byteHeader}");
        }
        decompressedStream.Position = 0;
        return decompressedStream;
    }

    private static INbt ReadNbtSwitch(NbtBinaryReader reader, NbtType nbtType, bool insideList)
    {
        string name = insideList ? "" : reader.ReadStringLengthPrefixed();
        reader.Stack.Push((nbtType, name));
        INbt nbt = nbtType switch
        {
            NbtType.NbtEnd => throw new NbtIllegalOperationException($"Cannot instantiate {NbtType.NbtEnd}"),
            NbtType.NbtByte => ReadNbtByte(reader, name),
            NbtType.NbtShort => ReadNbtShort(reader, name),
            NbtType.NbtInt => ReadNbtInt(reader, name),
            NbtType.NbtLong => ReadNbtLong(reader, name),
            NbtType.NbtFloat => ReadNbtFloat(reader, name),
            NbtType.NbtDouble => ReadNbtDouble(reader, name),
            NbtType.NbtString => ReadNbtString(reader, name),
            NbtType.NbtByteArray => ReadNbtByteArray(reader, name),
            NbtType.NbtIntArray => ReadNbtIntArray(reader, name),
            NbtType.NbtLongArray => ReadNbtLongArray(reader, name),
            NbtType.NbtCompound => ReadNbtCompound(reader, name),
            NbtType.NbtList => ReadNbtListSwitch(reader, name),
            _ => throw new NbtIllegalTypeException()
        };
        reader.Stack.Pop();
        return nbt;
    }

    private static NbtType ReadNbtType(NbtBinaryReader reader)
    {
        int type = reader.ReadByte();
        if (Enum.IsDefined(typeof(NbtType), type))
            return (NbtType)type;
        else
            throw new NbtIllegalTypeException(
                $"Illegal nbt type '{type}' at stream position {reader.Position}");
    }

    private static NbtByte ReadNbtByte(NbtBinaryReader reader, string name)
    {
        sbyte value = reader.ReadSByte();
        return new NbtByte(name, value);
    }

    private static NbtShort ReadNbtShort(NbtBinaryReader reader, string name)
    {
        short value = reader.ReadShortBE();
        return new NbtShort(name, value);
    }

    private static NbtInt ReadNbtInt(NbtBinaryReader reader, string name)
    {
        int value = reader.ReadIntBE();
        return new NbtInt(name, value);
    }

    private static NbtLong ReadNbtLong(NbtBinaryReader reader, string name)
    {
        long value = reader.ReadLongBE();
        return new NbtLong(name, value);
    }

    private static NbtFloat ReadNbtFloat(NbtBinaryReader reader, string name)
    {
        float value = reader.ReadFloatBE();
        return new NbtFloat(name, value);
    }

    private static NbtDouble ReadNbtDouble(NbtBinaryReader reader, string name)
    {
        double value = reader.ReadDoubleBE();
        return new NbtDouble(name, value);
    }

    private static NbtString ReadNbtString(NbtBinaryReader reader, string name)
    {
        string value = reader.ReadStringLengthPrefixed();
        return new NbtString(name, value);
    }

    private static NbtByteArray ReadNbtByteArray(NbtBinaryReader reader, string name)
    {
        int arrayLength = reader.ReadIntBE();
        sbyte[] array = new sbyte[arrayLength];
        for (int i = 0; i < array.Length; i++)
            array[i] = reader.ReadSByte();
        return new NbtByteArray(name, array);
    }

    private static NbtIntArray ReadNbtIntArray(NbtBinaryReader reader, string name)
    {
        int arrayLength = reader.ReadIntBE();
        int[] array = new int[arrayLength];
        for (int i = 0; i < array.Length; i++)
            array[i] = reader.ReadIntBE();
        return new NbtIntArray(name, array);
    }

    private static NbtLongArray ReadNbtLongArray(NbtBinaryReader reader, string name)
    {
        int arrayLength = reader.ReadIntBE();
        long[] array = new long[arrayLength];
        for (int i = 0; i < array.Length; i++)
            array[i] = reader.ReadLongBE();
        return new NbtLongArray(name, array);
    }

    private static NbtCompound ReadNbtCompound(NbtBinaryReader reader, string name)
    {
        NbtCompound nbtCompound = new(name);
        while (true)
        {
            NbtType nbtType = ReadNbtType(reader);
            if (nbtType == NbtType.NbtEnd)
                break;
            INbt nbt = ReadNbtSwitch(reader, nbtType, insideList: false);
            nbtCompound.Add(nbt.Name, nbt);
        }
        return nbtCompound;
    }

    private static INbt ReadNbtListSwitch(NbtBinaryReader reader, string name)
    {
        NbtType listType = ReadNbtType(reader);
        return listType switch
        {
            NbtType.NbtEnd => ReadNbtList<NbtByte>(reader, name, listType), // NbtEnd does not exist
            NbtType.NbtByte => ReadNbtList<NbtByte>(reader, name, listType),
            NbtType.NbtShort => ReadNbtList<NbtShort>(reader, name, listType),
            NbtType.NbtInt => ReadNbtList<NbtInt>(reader, name, listType),
            NbtType.NbtLong => ReadNbtList<NbtLong>(reader, name, listType),
            NbtType.NbtFloat => ReadNbtList<NbtFloat>(reader, name, listType),
            NbtType.NbtDouble => ReadNbtList<NbtDouble>(reader, name, listType),
            NbtType.NbtString => ReadNbtList<NbtString>(reader, name, listType),
            NbtType.NbtByteArray => ReadNbtList<NbtByteArray>(reader, name, listType),
            NbtType.NbtIntArray => ReadNbtList<NbtIntArray>(reader, name, listType),
            NbtType.NbtLongArray => ReadNbtList<NbtLongArray>(reader, name, listType),
            NbtType.NbtList => ReadNestedNbtList(reader, name),
            NbtType.NbtCompound => ReadNbtList<NbtCompound>(reader, name, listType),
            _ => throw new NbtIllegalTypeException()
        };
    }

    private static INbt ReadNestedNbtList(NbtBinaryReader reader, string name)
    {
        /* Reading nested NbtList is tricky because we can't determine what the
         * type of the NbtList is (can't instantiate, undefined type argument)
         * until we actually read what items that instance of NbtList is holding.
         *
         * To solve this, we have to polymorph it to its top-level inheritance, 
         * which is the interface of INbt in this case.
         * 
         * Technically we can inline the call in ReadNbtListSwitch at switch NbtType.NbtList
         * with => ReadNbtList<INbt>, but else we cannot make a documentation comment like this :)
         */

        // return ReadNbtList<???>(reader, name, NbtType.NbtList);
        // Here the ??? of T is Polymorphed to INbt.
        // OOP can be very powerful if done properly
        return ReadNbtList<INbt>(reader, name, NbtType.NbtList);
    }

    private static NbtList<T> ReadNbtList<T>(NbtBinaryReader reader, string name, NbtType listType) where T : class, INbt
    {
        int listLength = reader.ReadIntBE();
        NbtList<T> nbtList = new(name);
        for (int i = 0; i < listLength; i++)
        {
            T nbt = (ReadNbtSwitch(reader, listType, insideList: true) as T)!;
            nbtList.Add(nbt);
        }
        return nbtList;
    }
}
