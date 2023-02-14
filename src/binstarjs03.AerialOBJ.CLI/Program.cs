using System;
using System.Buffers.Binary;
using System.Runtime.InteropServices;
using System.Text;

namespace binstarjs03.AerialOBJ.CLI;

internal class Program
{
    [DllImport("binstarjs03.AerialOBJ.CoreNative.dll")]
    static extern void HelloWorld();

    [DllImport("binstarjs03.AerialOBJ.CoreNative.dll")]
    static extern void PrintString(IntPtr ptr, ushort length);

    static void Main(string[] args)
    {
        System.Console.WriteLine("Hello, World from CLR!");
        HelloWorld();

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
