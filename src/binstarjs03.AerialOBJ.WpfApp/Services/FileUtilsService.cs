using System.IO;

namespace binstarjs03.AerialOBJ.WpfApp.Services;
public class FileUtilsService : IFileUtilsService
{
    public bool Exist(string path) => File.Exists(path);
    public string ReadAllText(string path) => File.ReadAllText(path);
}
