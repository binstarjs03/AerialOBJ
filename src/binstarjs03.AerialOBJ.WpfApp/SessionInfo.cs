using System.IO;

using binstarjs03.AerialOBJ.Core.NbtNew;

namespace binstarjs03.AerialOBJ.WpfApp;

public class SessionInfo
{
    public SessionInfo(DirectoryInfo savegameDirectory, NbtCompound nbtLevel)
    {
        WorldName = nbtLevel.Get<NbtCompound>("Data")
                            .Get<NbtString>("LevelName")
                            .Value;
        SavegameDirectory = savegameDirectory;
    }

    public string WorldName { get; }

    public DirectoryInfo SavegameDirectory { get; }
}
