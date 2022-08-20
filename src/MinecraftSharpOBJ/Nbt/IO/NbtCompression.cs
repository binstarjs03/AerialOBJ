using System;
using System.IO;
using System.IO.Compression;
namespace binstarjs03.MinecraftSharpOBJ.Nbt.IO;

public static class NbtCompression {
    public enum ByteHeader {
        Gzip = 0x1F,
        Zlib = 0x78,
        TagCompound = 0x0A,
    }

    public enum Method {
        AutoDetect = -1,
        Uncompressed = 0,
        Gzip = 1,
        Zlib = 2
    }

    /// <exception cref="EndOfStreamException"></exception>
    public static Method DetectCompression(BinaryReader reader) {
        int byteHeader = reader.ReadByte();
        if (byteHeader == -1)
            throw new EndOfStreamException();
        reader.BaseStream.Seek(-1, SeekOrigin.Current);
        return DetectCompression((byte)byteHeader);
    }

    /// <exception cref="NbtUnknownCompressionMethodException"></exception>
    public static Method DetectCompression(byte byteHeader) {
        return byteHeader switch {
            (int)ByteHeader.Gzip => Method.Gzip,
            (int)ByteHeader.Zlib => Method.Zlib,
            (int)ByteHeader.TagCompound => Method.Uncompressed,
            _ => throw new NbtUnknownCompressionMethodException($"Unknown compression format of byte header {byteHeader}."),
        };
    }

    /// <exception cref="NbtUnknownCompressionMethodException"></exception>
    public static MemoryStream DecompressStream(Stream stream, Method compressionMethod) {
        MemoryStream memoryDecompressed = new();

        if (compressionMethod == Method.AutoDetect)
            compressionMethod = DetectCompression(new BinaryReader(stream));
        switch (compressionMethod) {
            case Method.Gzip:
                using (GZipStream decompressorGzip = new(stream, CompressionMode.Decompress))
                    decompressorGzip.CopyTo(memoryDecompressed);
                break;
            case Method.Zlib:
                using (ZLibStream decompressorZlib = new(stream, CompressionMode.Decompress))
                    decompressorZlib.CopyTo(memoryDecompressed);
                break;
            case Method.Uncompressed:
                using (stream)
                    stream.CopyTo(memoryDecompressed);
                break;
            default:
                throw new NbtUnknownCompressionMethodException($"Undefined compression method of {compressionMethod}");
        }
        memoryDecompressed.Seek(0, SeekOrigin.Begin);
        return memoryDecompressed;
    }
}
