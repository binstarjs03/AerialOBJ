using System;
using System.IO;
using System.IO.Compression;

namespace binstarjs03.AerialOBJ.Core.NbtNew;

public static class NbtIO
{
    public static INbt ReadDisk(string path)
    {
        return NbtReader.ReadDisk(path);
    }

    public static INbt ReadStream(Stream inputStream)
    {
        return NbtReader.ReadStream(inputStream);
    }

    public static void WriteDisk(string path, INbt nbt)
    {
        NbtWriter.WriteDisk(path, nbt);
    }

    public static void WriteStream(Stream outputStream, INbt nbt)
    {
        NbtWriter.WriteStream(outputStream, nbt);
    }
}