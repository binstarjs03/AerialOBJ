using System.IO.Compression;
using System.IO;

namespace binstarjs03.AerialOBJ.Core.NbtNew;
public static class NbtCompression
{
    private enum ByteHeader
    {
        Gzip = 0x1F,
        Zlib = 0x78,
        TagCompound = 0x0A,
    }

    public static MemoryStream DecompressStream(Stream rawStream)
    {
        MemoryStream decompressedStream;
        int byteHeader = rawStream.ReadByte();
        rawStream.Seek(-1, SeekOrigin.Current);
        switch (byteHeader)
        {
            case (int)ByteHeader.Gzip:
                using (GZipStream gZipStream = new(rawStream, CompressionMode.Decompress))
                {
                    decompressedStream = new MemoryStream();
                    gZipStream.CopyTo(decompressedStream);
                }
                break;
            case (int)ByteHeader.Zlib:
                using (ZLibStream zLibStream = new(rawStream, CompressionMode.Decompress))
                {
                    decompressedStream = new MemoryStream();
                    zLibStream.CopyTo(decompressedStream);
                }
                break;
            case (int)ByteHeader.TagCompound: // uncompressed nbt data
                // stream may be any instance of stream type,
                // if it's memory stream, just reuse that instance instead
                if (rawStream is MemoryStream rawMemoryStream)
                    decompressedStream = rawMemoryStream;
                else
                {
                    decompressedStream = new MemoryStream();
                    rawStream.CopyTo(decompressedStream);
                }
                break;
            default:
                throw new NbtUnknownCompressionMethodException(
                $"Undefined compression byte header of {byteHeader}");
        }
        decompressedStream.Position = 0;
        return decompressedStream;
    }
}
