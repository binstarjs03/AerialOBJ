using System;
using System.IO;
using System.IO.Compression;

namespace binstarjs03.AerialOBJ.Core.NbtFormat;

public static class NbtCompression
{
    private enum ByteHeader
    {
        Gzip = 0x1F, // 31
        Zlib = 0x78, // 120
        TagCompound = 0x0A, // 10
    }

    /// <summary>
    /// Decompress <paramref name="stream"/> and copies it to the returned <see cref="MemoryStream"/>.
    /// If <paramref name="stream"/> is an instance of <see cref="MemoryStream"/> and is uncompressed,
    /// it will return <paramref name="stream"/>
    /// </summary>
    /// <param name="stream">The stream that contains nbt data to be decompressed</param>
    /// <returns>Decompressed nbt stream</returns>
    /// <exception cref="NbtDecompressionException">When exception occured during decompression</exception>
    public static MemoryStream DecompressStream(Stream stream)
    {
        MemoryStream decompressedStream;
        int byteHeader = stream.ReadByte();
        stream.Seek(-1, SeekOrigin.Current);
        try
        {
            switch ((ByteHeader)byteHeader)
            {
                case ByteHeader.Gzip:
                    using (GZipStream gZipStream = new(stream, CompressionMode.Decompress))
                    {
                        decompressedStream = new MemoryStream();
                        gZipStream.CopyTo(decompressedStream);
                    }
                    break;
                case ByteHeader.Zlib:
                    using (ZLibStream zLibStream = new(stream, CompressionMode.Decompress))
                    {
                        decompressedStream = new MemoryStream();
                        zLibStream.CopyTo(decompressedStream);
                    }
                    break;
                case ByteHeader.TagCompound:
                    if (stream is MemoryStream memStream)
                        decompressedStream = memStream;
                    else
                    {
                        decompressedStream = new MemoryStream();
                        stream.CopyTo(decompressedStream);
                    }
                    break;
                default:
                    throw new NbtUnknownCompressionSchemeException(
                    $"Unknown compression byte header of {byteHeader}");
            }
            decompressedStream.Position = 0;
            return decompressedStream;

        }
        catch (Exception e) { throw new NbtDecompressionException("Exception occured during decompression of Nbt data", e); }
    }
}


public class NbtDecompressionException : Exception
{
    public NbtDecompressionException() { }
    public NbtDecompressionException(string message) : base(message) { }
    public NbtDecompressionException(string message, Exception inner) : base(message, inner) { }
}