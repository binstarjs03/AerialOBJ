namespace binstarjs03.AerialOBJ.WpfApp.Services;
public interface IFileUtilsService
{
    bool Exist(string path);
    string ReadAllText(string path);
}
