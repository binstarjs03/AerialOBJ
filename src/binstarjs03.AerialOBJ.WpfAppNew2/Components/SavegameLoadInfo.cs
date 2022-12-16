using System.IO;

namespace binstarjs03.AerialOBJ.WpfAppNew2.Components;
public class SavegameLoadInfo
{
    public string WorldName { get; }
    public DirectoryInfo SavegameDirectory { get; }
    
    public SavegameLoadInfo(string worldName, DirectoryInfo savegameDirectory)
    {
        WorldName = worldName;
        SavegameDirectory = savegameDirectory;
    }
}
