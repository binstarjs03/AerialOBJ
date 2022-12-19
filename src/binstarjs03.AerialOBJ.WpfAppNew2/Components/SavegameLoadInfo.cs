using System.IO;

namespace binstarjs03.AerialOBJ.WpfAppNew2.Components;
public class SavegameLoadInfo
{
    public string WorldName { get; }
    public string SavegameDirectoryPath { get; }
    
    public SavegameLoadInfo(string worldName, string savegameDirectoryPath)
    {
        WorldName = worldName;
        SavegameDirectoryPath = savegameDirectoryPath;
    }
}
