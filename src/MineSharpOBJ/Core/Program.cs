using System;
using binstarjs03.MineSharpOBJ.Core.Nbt;
using binstarjs03.MineSharpOBJ.Core.Nbt.Abstract;
using binstarjs03.MineSharpOBJ.Core.Nbt.Concrete;
namespace binstarjs03.MineSharpOBJ.Core;

internal class Program {
    static void Main(string[] args) {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.WriteLine(NbtType.NbtString);
        Console.ReadKey();
    }
}
