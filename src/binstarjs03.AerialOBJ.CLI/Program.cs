using System.Runtime.InteropServices;

namespace binstarjs03.AerialOBJ.CLI;

internal class Program
{
    [DllImport("binstarjs03.AerialOBJ.CoreNative.dll")]
    static extern void HelloWorld();

    static void Main(string[] args)
    {
        System.Console.WriteLine("Hello, World from CLR!");
        HelloWorld();
        System.Console.ReadKey();
    }
}
