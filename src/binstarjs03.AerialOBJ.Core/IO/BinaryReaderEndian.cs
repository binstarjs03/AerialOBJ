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
using System.Buffers.Binary;
using System.IO;
using System.Text;

namespace binstarjs03.AerialOBJ.Core.IO;

public class BinaryReaderEndian : BinaryReader
{
    public BinaryReaderEndian(Stream input) : base(input) { }
    public BinaryReaderEndian(Stream input, Encoding encoding) : base(input, encoding) { }
    public BinaryReaderEndian(Stream input, Encoding encoding, bool leaveOpen) : base(input, encoding, leaveOpen) { }

    public short ReadShort(ByteOrder byteOrder)
    {
        int readLength = sizeof(short);
        Span<byte> buffer = stackalloc byte[sizeof(short)];
        if (Read(buffer) != readLength)
            throw new EndOfStreamException();
        if (byteOrder == ByteOrder.BigEndian)
            return BinaryPrimitives.ReadInt16BigEndian(buffer);
        return BinaryPrimitives.ReadInt16LittleEndian(buffer);
    }

    public ushort ReadUShort(ByteOrder byteOrder)
    {
        int readLength = sizeof(short);
        Span<byte> buffer = stackalloc byte[readLength];
        if (Read(buffer) != readLength)
            throw new EndOfStreamException();
        if (byteOrder == ByteOrder.BigEndian)
            return BinaryPrimitives.ReadUInt16BigEndian(buffer);
        return BinaryPrimitives.ReadUInt16LittleEndian(buffer);
    }

    public int ReadInt(ByteOrder byteOrder)
    {
        int readLength = sizeof(int);
        Span<byte> buffer = stackalloc byte[readLength];
        if (Read(buffer) != readLength)
            throw new EndOfStreamException();
        if (byteOrder == ByteOrder.BigEndian)
            return BinaryPrimitives.ReadInt32BigEndian(buffer);
        return BinaryPrimitives.ReadInt32LittleEndian(buffer);
    }

    public long ReadLong(ByteOrder byteOrder)
    {
        int readLength = sizeof(long);
        Span<byte> buffer = stackalloc byte[readLength];
        if (Read(buffer) != readLength)
            throw new EndOfStreamException();
        if (byteOrder == ByteOrder.BigEndian)
            return BinaryPrimitives.ReadInt64BigEndian(buffer);
        return BinaryPrimitives.ReadInt64LittleEndian(buffer);
    }

    public float ReadFloat(ByteOrder byteOrder)
    {
        int readLength = sizeof(float);
        Span<byte> buffer = stackalloc byte[readLength];
        if (Read(buffer) != readLength)
            throw new EndOfStreamException();
        if (byteOrder == ByteOrder.BigEndian)
            return BinaryPrimitives.ReadSingleBigEndian(buffer);
        return BinaryPrimitives.ReadSingleLittleEndian(buffer);

    }

    public double ReadDouble(ByteOrder byteOrder)
    {
        int readLength = sizeof(double);
        Span<byte> buffer = stackalloc byte[readLength];
        if (Read(buffer) != readLength)
            throw new EndOfStreamException();
        if (byteOrder == ByteOrder.BigEndian)
            return BinaryPrimitives.ReadDoubleBigEndian(buffer);
        return BinaryPrimitives.ReadDoubleLittleEndian(buffer);
    }

    public string ReadStringLengthPrefixed(ByteOrder byteOrder)
    {
        ushort length = ReadUShort(byteOrder);
        Span<byte> bytes = length < 1024 ? stackalloc byte[length] : new byte[length];
        if (Read(bytes) != length)
            throw new EndOfStreamException();
        return Encoding.UTF8.GetString(bytes);
    }
}
