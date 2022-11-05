using System.IO;

using binstarjs03.AerialOBJ.Core.NbtNew;

namespace binstarjs03.AerialOBJ.WpfApp;

// TODO we could cache many things here such as list of valid region files etc
public class SavegameLoadInfo
{
    public string WorldName { get; }
    public DirectoryInfo SavegameDirectory { get; }

    public SavegameLoadInfo(DirectoryInfo savegameDirectory, NbtCompound nbtLevel)
    {
        WorldName = nbtLevel.Get<NbtCompound>("Data")
                            .Get<NbtString>("LevelName")
                            .Value;
        SavegameDirectory = savegameDirectory;
    }
}
