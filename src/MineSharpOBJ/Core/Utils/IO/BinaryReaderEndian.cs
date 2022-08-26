using System;
using System.IO;
using System.Buffers.Binary;
using System.Linq;
using System.Text;
namespace binstarjs03.MineSharpOBJ.Core.Utils.IO;

/// <summary>
/// Wrapper around <see cref="BinaryReader"/> that is endian-aware.
/// </summary>
public class BinaryReaderEndian : IDisposable {
    private static readonly string _disposedExceptionMsg = "Cannot read data, reader is already disposed";
    
    protected readonly Stream _baseStream;
    protected readonly BinaryReader _reader;
    protected readonly ByteOrder _byteOrder;
    private bool _hasDisposed;

    /// <exception cref="IOException"></exception>
    public BinaryReaderEndian(Stream input, ByteOrder ByteOrder) {
        if (!input.CanRead)
            throw new IOException("Input stream is unreadable (may be closed/disposed or write-only)");
        _baseStream = input;
        _reader = new BinaryReader(input);
        _byteOrder = ByteOrder;
        _hasDisposed = false;
    }

    public Stream BaseStream {
        get { return _baseStream; }
    }

    public BinaryReader Reader {
        get { return _reader; }
    }

    protected virtual void Dispose(bool disposing) {
        if (!_hasDisposed) {
            if (disposing) {
                _reader.Dispose();
                _baseStream.Dispose();
            }
            _hasDisposed = true;
        }
    }

    public void Dispose() {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <exception cref="EndOfStreamException"></exception>
    /// <exception cref="ObjectDisposedException"></exception>
    /// <exception cref="IOException"></exception>
    public byte[] ReadBytes(int length, bool endianMatter = true) {
        byte[] buff = new byte[length];
        if (_hasDisposed)
            throw new ObjectDisposedException(nameof(_reader), _disposedExceptionMsg);
        if (_reader.Read(buff) != length)
            throw new EndOfStreamException();
        if (_byteOrder == ByteOrder.BigEndian && endianMatter)
            return buff.Reverse().ToArray();
        return buff;
    }

    /// <exception cref="EndOfStreamException"></exception>
    /// <exception cref="ObjectDisposedException"></exception>
    /// <exception cref="IOException"></exception>
    public byte ReadByte() {
        if (_hasDisposed)
            throw new ObjectDisposedException(nameof(_reader), _disposedExceptionMsg);
        return _reader.ReadByte();
    }

    /// <exception cref="EndOfStreamException"></exception>
    /// <exception cref="ObjectDisposedException"></exception>
    /// <exception cref="IOException"></exception>
    public sbyte ReadSByte() {
        if (_hasDisposed)
            throw new ObjectDisposedException(nameof(_reader), _disposedExceptionMsg);
        return _reader.ReadSByte();
    }

    /// <exception cref="EndOfStreamException"></exception>
    /// <exception cref="ObjectDisposedException"></exception>
    /// <exception cref="IOException"></exception>
    public short ReadShort() {
        return BinaryPrimitives.ReadInt16LittleEndian(ReadBytes(sizeof(short)));
    }

    /// <exception cref="EndOfStreamException"></exception>
    /// <exception cref="ObjectDisposedException"></exception>
    /// <exception cref="IOException"></exception>
    public ushort ReadUShort() {
        return BinaryPrimitives.ReadUInt16LittleEndian(ReadBytes(sizeof(ushort)));
    }

    /// <exception cref="EndOfStreamException"></exception>
    /// <exception cref="ObjectDisposedException"></exception>
    /// <exception cref="IOException"></exception>
    public int ReadInt() {
        return BinaryPrimitives.ReadInt32LittleEndian(ReadBytes(sizeof(int)));
    }

    /// <exception cref="EndOfStreamException"></exception>
    /// <exception cref="ObjectDisposedException"></exception>
    /// <exception cref="IOException"></exception>
    public uint ReadUInt() {
        return BinaryPrimitives.ReadUInt32LittleEndian(ReadBytes(sizeof(uint)));
    }

    /// <exception cref="EndOfStreamException"></exception>
    /// <exception cref="ObjectDisposedException"></exception>
    /// <exception cref="IOException"></exception>
    public long ReadLong() {
        return BinaryPrimitives.ReadInt64LittleEndian(ReadBytes(sizeof(long)));
    }

    /// <exception cref="EndOfStreamException"></exception>
    /// <exception cref="ObjectDisposedException"></exception>
    /// <exception cref="IOException"></exception>
    public ulong ReadULong() {
        return BinaryPrimitives.ReadUInt64LittleEndian(ReadBytes(sizeof(ulong)));
    }

    /// <exception cref="EndOfStreamException"></exception>
    /// <exception cref="ObjectDisposedException"></exception>
    /// <exception cref="IOException"></exception>
    public float ReadFloat() {
        return BinaryPrimitives.ReadSingleLittleEndian(ReadBytes(sizeof(float)));
    }

    /// <exception cref="EndOfStreamException"></exception>
    /// <exception cref="ObjectDisposedException"></exception>
    /// <exception cref="IOException"></exception>
    public double ReadDouble() {
        return BinaryPrimitives.ReadDoubleLittleEndian(ReadBytes(sizeof(double)));
    }

    /// <exception cref="EndOfStreamException"></exception>
    /// <exception cref="ObjectDisposedException"></exception>
    /// <exception cref="IOException"></exception>
    /// <exception cref="DecoderFallbackException"></exception>
    public string ReadString(int prefixByteLength) {
        int length = BinaryPrimitives.ReadInt16LittleEndian(ReadBytes(prefixByteLength));
        long originalPos = BaseStream.Position;
        byte[] chars = ReadBytes(length, endianMatter: false);
        long newPos = BaseStream.Position;
        try {
            return Encoding.UTF8.GetString(chars);
        }
        catch (DecoderFallbackException e) {
            string msg = "Failed to decode binary Nbt string "
                       + $"at steam {originalPos} through {newPos}. ";
            throw new DecoderFallbackException(msg, e);
        }
    }

}
