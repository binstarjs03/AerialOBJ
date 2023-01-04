using System.IO;

namespace binstarjs03.AerialOBJ.WpfApp.Services;
public class FileUtilsService : IFileUtilsService
{
    public bool Exist(string path) => File.Exists(path);
    public string ReadAllText(string path) => File.ReadAllText(path);
    public void Copy(string sourcePath, string destinationPath) => File.Copy(sourcePath, destinationPath);
    public void CreateDirectory(string path) => Directory.CreateDirectory(path);
}
