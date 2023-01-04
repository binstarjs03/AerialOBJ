namespace binstarjs03.AerialOBJ.WpfApp.Services;
public interface IFileUtilsService
{
    bool Exist(string path);
    string ReadAllText(string path);
    void Copy(string sourcePath, string destinationPath);
    void CreateDirectory(string path);
}
