using System;
using System.IO;
using System.Runtime.InteropServices;

using binstarjs03.AerialOBJ.Core.NbtFormat;

namespace binstarjs03.AerialOBJ.CLI;

internal class Program
{
    [DllImport("binstarjs03.AerialOBJ.CoreNative.dll")]
    static extern void HelloWorld();


    [DllImport("binstarjs03.AerialOBJ.CoreNative.dll")]
    static extern void ParseNbt(IntPtr ptr, uint length);

    static void Main(string[] args)
    {
        Console.WriteLine("Hello World from Managed!");
        HelloWorld();
        Console.WriteLine();

        ReadNBT();
    }

    static void ReadNBT()
    {
        // replace path with actual path where your level.dat is
        string path = @"C:\Users\...\AppData\Roaming\.minecraft\saves\...\level.dat";
        Stream stream = File.OpenRead(path);
        MemoryStream ms = NbtCompression.DecompressStream(stream);
        byte[] data = ms.ToArray();

        Console.WriteLine($"Reading NBT Native...");
        var start = DateTime.Now;
        ReadNBTNative(data);
        var duration = DateTime.Now - start;
        Console.WriteLine($"Finished reading nbt native. Duration: {duration.TotalMilliseconds} ms");

        // jitting
        ReadNBTManaged(data);
        ReadNBTManaged(data);
        ReadNBTManaged(data);
        // finished jitting

        Console.WriteLine($"Reading NBT Managed...");
        start = DateTime.Now;
        ReadNBTManaged(data);
        duration = DateTime.Now - start;
        Console.WriteLine($"Finished reading nbt managed. Duration: {duration.TotalMilliseconds} ms");
    }

    static void ReadNBTNative(byte[] data)
    {
        var gcHandle = GCHandle.Alloc(data, GCHandleType.Pinned);
        var ptr = gcHandle.AddrOfPinnedObject();
        ParseNbt(ptr, (uint)data.Length);
        gcHandle.Free();
    }

    static void ReadNBTManaged(byte[] data)
    {
        NbtIO.ReadStream(new MemoryStream(data));
    }
}
