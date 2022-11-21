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

// TODO implement the ability to read either BE/LE by passing enum
/// <summary>
/// Wrapper around <see cref="BinaryWriter"/> that takes endianness into account
/// </summary>
public class BinaryWriterEndian : IDisposable
{
    protected Stream _stream;
    protected BinaryWriter _writer;
    private bool _disposed = false;

    public BinaryWriterEndian(Stream stream)
    {
        _stream = stream;
        _writer = new BinaryWriter(stream);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _stream.Dispose();
                _writer.Dispose();
            }
            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public MemoryStream? AsMemoryStream()
    {
        return _stream as MemoryStream;
    }

    public void WriteByte(byte value)
    {
        _writer.Write(value);
    }

    public void WriteSByte(sbyte value)
    {
        _writer.Write(value);
    }

    public void WriteShortBE(short value)
    {
        Span<byte> buffer = stackalloc byte[sizeof(short)];
        BinaryPrimitives.WriteInt16BigEndian(buffer, value);
        _writer.Write(buffer);
    }

    public void WriteUShortBE(ushort value)
    {
        Span<byte> buffer = stackalloc byte[sizeof(ushort)];
        BinaryPrimitives.WriteUInt16BigEndian(buffer, value);
        _writer.Write(buffer);
    }

    public void WriteIntBE(int value)
    {
        Span<byte> buffer = stackalloc byte[sizeof(int)];
        BinaryPrimitives.WriteInt32BigEndian(buffer, value);
        _writer.Write(buffer);
    }

    public void WriteLongBE(long value)
    {
        Span<byte> buffer = stackalloc byte[sizeof(long)];
        BinaryPrimitives.WriteInt64BigEndian(buffer, value);
        _writer.Write(buffer);
    }

    public void WriteFloatBE(float value)
    {
        Span<byte> buffer = stackalloc byte[sizeof(float)];
        BinaryPrimitives.WriteSingleBigEndian(buffer, value);
        _writer.Write(buffer);
    }

    public void WriteDoubleBE(double value)
    {
        Span<byte> buffer = stackalloc byte[sizeof(double)];
        BinaryPrimitives.WriteDoubleBigEndian(buffer, value);
        _writer.Write(buffer);
    }

    public void WriteStringLengthPrefixed(string value)
    {
        int length = Encoding.UTF8.GetByteCount(value);
        if (length > ushort.MaxValue)
            throw new ArgumentOutOfRangeException(nameof(value), "String length is too long to write");

        // allocate buffer in stack if string size is less than 1KiB, else allocate in heap.
        // This is to guard from stackoverflow exception
        Span<byte> buff = length < 1024 ? stackalloc byte[length] : new byte[length];
        Encoding.UTF8.GetBytes(value, buff);
        WriteUShortBE((ushort)length);
        _writer.Write(buff);
    }
}
