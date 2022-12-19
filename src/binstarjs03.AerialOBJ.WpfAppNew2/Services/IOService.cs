using System;
using System.IO;

namespace binstarjs03.AerialOBJ.WpfAppNew2.Services;
public class IOService : IIOService
{
    public bool WriteText(string path, string content, out Exception? e)
    {
        try
        {
            using StreamWriter writer = File.CreateText(path);
            writer.Write(content);
            e = null;
            return true;
        }
        catch (Exception ex)
        {
            e = ex;
            return false;
        }
    }
}
