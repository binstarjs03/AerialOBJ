using System.IO;

namespace binstarjs03.AerialOBJ.MvvmAppCore.Services.IOService;
public class AbstractIO : IAbstractIO
{
    public void WriteText(string path, string content)
    {
        using StreamWriter writer = File.CreateText(path);
        writer.Write(content);
    }
}
