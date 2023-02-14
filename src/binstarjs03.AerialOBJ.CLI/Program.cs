using System;
using System.Buffers.Binary;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

using binstarjs03.AerialOBJ.Core.NbtFormat;

namespace binstarjs03.AerialOBJ.CLI;

internal class Program
{
    [DllImport("binstarjs03.AerialOBJ.CoreNative.dll")]
    static extern void HelloWorld();

    [DllImport("binstarjs03.AerialOBJ.CoreNative.dll")]
    static extern void PrintString(IntPtr ptr, ushort length);

    [DllImport("binstarjs03.AerialOBJ.CoreNative.dll")]
    static extern void ParseNbt(IntPtr ptr, uint length);

    static void Main(string[] args)
    {
        Console.WriteLine("Warming Up...");
        Thread.Sleep(500);
        Console.WriteLine("Go!");
        
        Console.WriteLine("Hello World from Managed!");
        HelloWorld();

        ReadNBT();
    }

    static void ReadNBT()
    {
        string path = @"C:\Users\Bin\AppData\Roaming\.minecraft\saves\Terralith 2\level.dat";
        Stream stream = File.OpenRead(path);
        MemoryStream ms = NbtCompression.DecompressStream(stream);
        byte[] data = ms.ToArray();

        //ReadNBTNative(data);
        //ReadNBTNative(data);
        Console.WriteLine($"Reading NBT Native...");
        var start = DateTime.Now;
        ReadNBTNative(data);
        var duration = DateTime.Now - start;
        Console.WriteLine($"Finished reading nbt native. Duration: {duration}");

        //ReadNBTManaged(data);
        //ReadNBTManaged(data);
        Console.WriteLine($"Reading NBT Managed...");
        start = DateTime.Now;
        ReadNBTManaged(data);
        duration = DateTime.Now - start;
        Console.WriteLine($"Finished reading nbt managed. Duration: {duration}");
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

    static void Run1()
    {
        string str = "Okii Dialogue Inc.";
        byte[] strbytes = Encoding.UTF8.GetBytes(str);
        byte[] finalBuff = new byte[2 + strbytes.Length];
        Span<byte> strbyteLengthBuff = stackalloc byte[2];
        BinaryPrimitives.WriteInt16LittleEndian(strbyteLengthBuff, (short)strbytes.Length);
        finalBuff[0] = strbyteLengthBuff[0];
        finalBuff[1] = strbyteLengthBuff[1];
        for (int j = 0; j < strbytes.Length; j++)
            finalBuff[2 + j] = strbytes[j];

        var gcHandle = GCHandle.Alloc(finalBuff, GCHandleType.Pinned);
        var ptr = gcHandle.AddrOfPinnedObject();

        PrintString(ptr, (ushort)finalBuff.Length);
    }
}
