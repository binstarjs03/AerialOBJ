using System.IO;

namespace binstarjs03.AerialOBJ.WpfApp.Services;
public class IOService : IIOService
{
    public void WriteText(string path, string content)
    {
        using StreamWriter writer = File.CreateText(path);
        writer.Write(content);
    }
}
