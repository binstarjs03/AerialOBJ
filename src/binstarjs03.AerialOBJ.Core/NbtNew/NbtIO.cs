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
}