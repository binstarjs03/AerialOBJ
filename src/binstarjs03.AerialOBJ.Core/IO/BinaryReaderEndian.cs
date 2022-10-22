using System;
using System.Buffers.Binary;
using System.IO;
using System.Linq;
using System.Text;

namespace binstarjs03.AerialOBJ.Core.IO;

/// <summary>
/// Wrapper around <see cref="BinaryReader"/> that is endian-aware.
/// </summary>
public class BinaryReaderEndian : IDisposable
{
    private static readonly string s_disposedExceptionMsg = "Cannot read data, reader is already disposed";
    protected readonly Stream _baseStream;
    protected readonly BinaryReader _reader;
    protected readonly ByteOrder _byteOrder;
    private bool _hasDisposed;

    public BinaryReaderEndian(Stream input, ByteOrder ByteOrder)
    {
        if (!input.CanRead)
            throw new ArgumentException("Input stream is unreadable (may be closed/disposed or write-only)");
        _baseStream = input;
        _reader = new BinaryReader(input);
        _byteOrder = ByteOrder;
        _hasDisposed = false;
    }

    public Stream BaseStream => _baseStream;

    public BinaryReader Reader => _reader;

    #region Dispose Pattern

    protected virtual void Dispose(bool disposing)
    {
        if (!_hasDisposed)
        {
            if (disposing)
            {
                _reader.Dispose();
                _baseStream.Dispose();
            }
            // dispose unmanaged object
            // set large fields to null
            _hasDisposed = true;
        }
    }

    // Destructor not implemented
    // cause there is no unmanaged object to dispose

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    #endregion

    public byte[] ReadBytes(int length, bool endianMatter = true)
    {
        byte[] buff = new byte[length];
        if (_hasDisposed)
            throw new ObjectDisposedException(nameof(_reader), s_disposedExceptionMsg);
        if (_reader.Read(buff) != length)
            throw new EndOfStreamException();
        if (_byteOrder == ByteOrder.BigEndian && endianMatter)
        {
            Array.Reverse(buff);
            return buff;
        }
        return buff;
    }

    public byte ReadByte()
    {
        return _reader.ReadByte();
    }

    public sbyte ReadSByte()
    {
        return _reader.ReadSByte();
    }

    public short ReadShort()
    {
        long originalPos = _baseStream.Position;
        long posAfterReadingLong = originalPos + sizeof(short);
        _baseStream.Position = posAfterReadingLong;
        short result = 0;
        for (int i = 0; i < sizeof(short); i++)
        {
            _baseStream.Position = posAfterReadingLong - 1 - i;
            int buff = _reader.ReadByte();
            buff = buff << (i) * 8;
            result += (short)buff;
        }
        _baseStream.Position = posAfterReadingLong;
        return result;
    }

    //public ushort ReadUShort()
    //{
    //    return BinaryPrimitives.ReadUInt16LittleEndian(ReadBytes(sizeof(ushort)));
    //}

    public int ReadInt()
    {
        long originalPos = _baseStream.Position;
        long posAfterReadingLong = originalPos + sizeof(int);
        _baseStream.Position = posAfterReadingLong;
        int result = 0;
        for (int i = 0; i < sizeof(int); i++)
        {
            _baseStream.Position = posAfterReadingLong - 1 - i;
            int buff = _reader.ReadByte();
            buff = buff << (i) * 8;
            result += buff;
        }
        _baseStream.Position = posAfterReadingLong;
        return result;
    }


    //public uint ReadUInt()
    //{
    //    return BinaryPrimitives.ReadUInt32LittleEndian(ReadBytes(sizeof(uint)));
    //}

    public long ReadLong()
    {
        // readable version, which is unoptimized: it allocates heap (array)
        //return BinaryPrimitives.ReadInt64LittleEndian(ReadBytes(sizeof(long)));

        // unreadable version, but is optimized: no heap allocation
        long originalPos = _baseStream.Position;
        long posAfterReadingLong = originalPos + sizeof(long);
        _baseStream.Position = posAfterReadingLong;
        long result = 0;
        for (int i = 0; i < sizeof(long); i++)
        {
            _baseStream.Position = posAfterReadingLong - 1 - i;
            long buff = _reader.ReadByte();
            buff = buff << (i) * 8;
            result += buff;
        }
        _baseStream.Position = posAfterReadingLong;
        return result;
    }

    //public ulong ReadULong()
    //{
    //    return BinaryPrimitives.ReadUInt64LittleEndian(ReadBytes(sizeof(ulong)));
    //}

    // float and double cannot be optimized to no-heap allocation
    public float ReadFloat()
    {
        return BinaryPrimitives.ReadSingleLittleEndian(ReadBytes(sizeof(float)));
    }

    public double ReadDouble()
    {
        return BinaryPrimitives.ReadDoubleLittleEndian(ReadBytes(sizeof(double)));
    }

    public string ReadString()
    {
        int length = ReadShort();
        long originalPos = BaseStream.Position;
        byte[] chars = ReadBytes(length, endianMatter: false);
        long newPos = BaseStream.Position;
        try
        {
            return Encoding.UTF8.GetString(chars);
        }
        catch (DecoderFallbackException e)
        {
            string msg = "Failed to decode binary Nbt string "
                       + $"at steam {originalPos} through {newPos}. ";
            throw new DecoderFallbackException(msg, e);
        }
    }

}
