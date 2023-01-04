using binstarjs03.AerialOBJ.WpfApp.Components;

namespace binstarjs03.AerialOBJ.WpfApp.Factories;
public class FileInfoFactory : IFileInfoFactory
{
    public IFileInfo Create(string path)
    {
        return new DefaultFileInfo(path);
    }
}
