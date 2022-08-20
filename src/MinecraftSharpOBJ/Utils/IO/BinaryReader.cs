using System;
using System.IO;
using System.Buffers.Binary;
using System.Linq;
using System.Text;
namespace binstarjs03.MinecraftSharpOBJ.Utils.IO;

/// <summary>
/// Wrapper around <see cref="System.IO.BinaryReader"/> that is endian-aware.
/// </summary>
public class BinaryReader : IDisposable {
    private static readonly string _disposedExceptionMsg = "Cannot read data, reader is already disposed";
    
    protected readonly Stream _baseStream;
    protected readonly System.IO.BinaryReader _reader;
    protected readonly ByteOrder _byteOrder;
    private bool _hasDisposed;

    /// <exception cref="IOException"></exception>
    public BinaryReader(Stream input, ByteOrder ByteOrder) {
        if (!input.CanRead)
            throw new IOException("Input stream is unreadable (may be closed/disposed or write-only)");
        _baseStream = input;
        _reader = new System.IO.BinaryReader(input);
        _byteOrder = ByteOrder;
        _hasDisposed = false;
    }

    public Stream BaseStream {
        get { return _baseStream; }
    }

    public System.IO.BinaryReader Reader {
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
    public byte[] ReadBytes(int length, bool endianMatter = true) {
        byte[] buff = new byte[length];
        if (_hasDisposed)
            throw new ObjectDisposedException(_disposedExceptionMsg);
        int readLength = _reader.Read(buff);
        if (readLength != length)
            throw new EndOfStreamException();
        if (_byteOrder == ByteOrder.BigEndian && endianMatter)
            return buff.Reverse().ToArray();
        return buff;
    }

    /// <exception cref="ObjectDisposedException"></exception>
    public byte ReadByte() {
        if (_hasDisposed)
            throw new ObjectDisposedException(_disposedExceptionMsg);
        return _reader.ReadByte();
    }

    /// <exception cref="ObjectDisposedException"></exception>
    public sbyte ReadSByte() {
        if (_hasDisposed)
            throw new ObjectDisposedException(_disposedExceptionMsg);
        return _reader.ReadSByte();
    }

    public short ReadShort() {
        return BinaryPrimitives.ReadInt16LittleEndian(ReadBytes(sizeof(short)));
    }

    public ushort ReadUShort() {
        return BinaryPrimitives.ReadUInt16LittleEndian(ReadBytes(sizeof(ushort)));
    }

    public int ReadInt() {
        return BinaryPrimitives.ReadInt32LittleEndian(ReadBytes(sizeof(int)));
    }

    public uint ReadUInt() {
        return BinaryPrimitives.ReadUInt32LittleEndian(ReadBytes(sizeof(uint)));
    }

    public long ReadLong() {
        return BinaryPrimitives.ReadInt64LittleEndian(ReadBytes(sizeof(long)));
    }

    public ulong ReadULong() {
        return BinaryPrimitives.ReadUInt64LittleEndian(ReadBytes(sizeof(ulong)));
    }

    public float ReadFloat() {
        return BinaryPrimitives.ReadSingleLittleEndian(ReadBytes(sizeof(float)));
    }

    public double ReadDouble() {
        return BinaryPrimitives.ReadDoubleLittleEndian(ReadBytes(sizeof(double)));
    }

    public string ReadString(int prefixByteLength) {
        int length = BinaryPrimitives.ReadInt16LittleEndian(ReadBytes(prefixByteLength));
        byte[] chars = ReadBytes(length, endianMatter: false);
        return Encoding.UTF8.GetString(chars);
    }

}
