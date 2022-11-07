﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace binstarjs03.AerialOBJ.Core.NbtNew;

public static class NbtWriter
{
    public static void WriteDisk(string path, INbt nbt)
    {
        using MemoryStream ms = new();
        WriteStream(ms, nbt);
        ms.Position = 0;
        using FileStream fs = File.OpenWrite(path);
        ms.CopyTo(fs);
        ms.Flush();
        fs.Flush();
    }

    public static void WriteStream(Stream outputStream, INbt nbt)
    {
        BinaryWriterEndian writer = new(outputStream);
        WriteNbtSwitch(writer, nbt, false);
    }

    private static void WriteNbtSwitch(BinaryWriterEndian writer, INbt nbt, bool insideList)
    {
        NbtType type = nbt.Type;
        if (!insideList)
        {
            WriteNbtType(writer, type);
            writer.WriteStringLengthPrefixed(nbt.Name);
        }
        switch (type)
        {
            case NbtType.NbtByte:
                WriteNbtByte(writer, (nbt as NbtByte)!);
                break;
            case NbtType.NbtShort:
                WriteNbtShort(writer, (nbt as NbtShort)!);
                break;
            case NbtType.NbtInt:
                WriteNbtInt(writer, (nbt as NbtInt)!);
                break;
            case NbtType.NbtLong:
                WriteNbtLong(writer, (nbt as NbtLong)!);
                break;
            case NbtType.NbtFloat:
                WriteNbtFloat(writer, (nbt as NbtFloat)!);
                break;
            case NbtType.NbtDouble:
                WriteNbtDouble(writer, (nbt as NbtDouble)!);
                break;
            case NbtType.NbtString:
                WriteNbtString(writer, (nbt as NbtString)!);
                break;
            case NbtType.NbtByteArray:
                WriteNbtByteArray(writer, (nbt as NbtByteArray)!);
                break;
            case NbtType.NbtIntArray:
                WriteNbtIntArray(writer, (nbt as NbtIntArray)!);
                break;
            case NbtType.NbtLongArray:
                WriteNbtLongArray(writer, (nbt as NbtLongArray)!);
                break;
            case NbtType.NbtCompound:
                WriteNbtCompound(writer, (nbt as NbtCompound)!);
                break;
            case NbtType.NbtList:
                WriteNbtList(writer, (nbt as NbtList<INbt>)!);
                break;
            default:
                throw new NotImplementedException();
        }
    }

    private static void WriteNbtType(BinaryWriterEndian writer, NbtType type)
    {
        if ((int)type < 0)
            throw new NbtIllegalOperationException("Cannot write AerialOBJ-specific Nbt type");
        writer.WriteByte((byte)type);
    }

    private static void WriteNbtByte(BinaryWriterEndian writer, NbtByte nbt)
    {
        writer.WriteSByte(nbt.Value);
    }

    private static void WriteNbtShort(BinaryWriterEndian writer, NbtShort nbt)
    {
        writer.WriteShortBE(nbt.Value);
    }

    private static void WriteNbtInt(BinaryWriterEndian writer, NbtInt nbt)
    {
        writer.WriteIntBE(nbt.Value);
    }

    private static void WriteNbtLong(BinaryWriterEndian writer, NbtLong nbt)
    {
        writer.WriteLongBE(nbt.Value);
    }

    private static void WriteNbtFloat(BinaryWriterEndian writer, NbtFloat nbt)
    {
        writer.WriteFloatBE(nbt.Value);
    }

    private static void WriteNbtDouble(BinaryWriterEndian writer, NbtDouble nbt)
    {
        writer.WriteDoubleBE(nbt.Value);
    }

    private static void WriteNbtString(BinaryWriterEndian writer, NbtString nbt)
    {
        writer.WriteStringLengthPrefixed(nbt.Value);
    }

    private static void WriteNbtByteArray(BinaryWriterEndian writer, NbtByteArray nbt)
    {
        writer.WriteIntBE(nbt.Values.Length);
        foreach (sbyte value in nbt.Values)
        {
            writer.WriteSByte(value);
        }
    }

    private static void WriteNbtIntArray(BinaryWriterEndian writer, NbtIntArray nbt)
    {
        writer.WriteIntBE(nbt.Values.Length);
        foreach (int value in nbt.Values)
        {
            writer.WriteIntBE(value);
        }
    }

    private static void WriteNbtLongArray(BinaryWriterEndian writer, NbtLongArray nbt)
    {
        writer.WriteIntBE(nbt.Values.Length);
        foreach (long value in nbt.Values)
        {
            writer.WriteLongBE(value);
        }
    }

    private static void WriteNbtCompound(BinaryWriterEndian writer, NbtCompound nbt)
    {
        foreach (INbt subNbt in nbt.Values)
            WriteNbtSwitch(writer, subNbt, false);
        WriteNbtType(writer, NbtType.NbtEnd);
    }

    private static void WriteNbtList<TNbt>(BinaryWriterEndian writer, NbtList<TNbt> nbt) where TNbt : class, INbt
    {
        NbtType listType = nbt.ListType;
        if (listType == NbtType.InvalidOrUnknown)
            // AerialOBJ permitted the use of mixed-type of list nbt through polymorphism
            // (via ascending hierarchy type) but the notchian implementation is illegal for this
            throw new NbtIllegalOperationException($"Cannot write mixed-type of {nameof(NbtList<INbt>)} nbt");
        else if (nbt.Count <= 0)
            // Notchian implementation use list type of end tag for zero-length list
            listType = NbtType.NbtEnd; 
        WriteNbtType(writer, listType);
        writer.WriteIntBE(nbt.Count);
        foreach (INbt subNbt in nbt)
        {
            WriteNbtSwitch(writer, subNbt, true);
        }
    }
}