using System;
using System.Buffers.Binary;
using System.IO;
using System.Text;

namespace binstarjs03.AerialOBJ.Core;

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
