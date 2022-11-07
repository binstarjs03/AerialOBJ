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

namespace binstarjs03.AerialOBJ.Core;

// TODO implement the ability to read either BE/LE by passing enum
/// <summary>
/// Wrapper around <see cref="BinaryReader"/> that takes endianness into account
/// </summary>
public class BinaryReaderEndian : IDisposable
{
    protected Stream _stream;
    protected BinaryReader _reader;
    private bool _disposed = false;

    public long Position => _stream.Position;

    public BinaryReaderEndian(Stream stream)
    {
        _stream = stream;
        _reader = new BinaryReader(_stream);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _reader.Dispose();
                _stream.Dispose();
            }
            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public byte ReadByte()
    {
        return _reader.ReadByte();
    }

    public sbyte ReadSByte()
    {
        return _reader.ReadSByte();
    }

    public short ReadShortBE()
    {
        int readLength = sizeof(short);
        Span<byte> buffer = stackalloc byte[readLength];
        if (_reader.Read(buffer) != readLength)
            throw new EndOfStreamException();
        return BinaryPrimitives.ReadInt16BigEndian(buffer);
    }

    public ushort ReadUShortBE()
    {
        int readLength = sizeof(short);
        Span<byte> buffer = stackalloc byte[readLength];
        if (_reader.Read(buffer) != readLength)
            throw new EndOfStreamException();
        return BinaryPrimitives.ReadUInt16BigEndian(buffer);
    }

    public int ReadIntBE()
    {
        int readLength = sizeof(int);
        Span<byte> buffer = stackalloc byte[readLength];
        if (_reader.Read(buffer) != readLength)
            throw new EndOfStreamException();
        return BinaryPrimitives.ReadInt32BigEndian(buffer);
    }

    public long ReadLongBE()
    {
        int readLength = sizeof(long);
        Span<byte> buffer = stackalloc byte[readLength];
        if (_reader.Read(buffer) != readLength)
            throw new EndOfStreamException();
        return BinaryPrimitives.ReadInt64BigEndian(buffer);
    }

    public float ReadFloatBE()
    {
        int readLength = sizeof(float);
        Span<byte> buffer = stackalloc byte[readLength];
        if (_reader.Read(buffer) != readLength)
            throw new EndOfStreamException();
        return BinaryPrimitives.ReadSingleBigEndian(buffer);
    }

    public double ReadDoubleBE()
    {
        int readLength = sizeof(double);
        Span<byte> buffer = stackalloc byte[readLength];
        if (_reader.Read(buffer) != readLength)
            throw new EndOfStreamException();
        return BinaryPrimitives.ReadDoubleBigEndian(buffer);
    }

    public string ReadStringLengthPrefixed()
    {
        ushort length = ReadUShortBE();
        Span<byte> bytes = stackalloc byte[length];
        if (_reader.Read(bytes) != length)
            throw new EndOfStreamException();
        return Encoding.UTF8.GetString(bytes);
    }
}
