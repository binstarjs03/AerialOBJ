using System;
using System.Buffers.Binary;
using System.IO;
using System.Text;

namespace binstarjs03.AerialOBJ.Core;

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
        int length = ReadShortBE();
        Span<byte> bytes = stackalloc byte[length];
        if (_reader.Read(bytes) != length)
            throw new EndOfStreamException();
        return Encoding.UTF8.GetString(bytes);
    }
}
