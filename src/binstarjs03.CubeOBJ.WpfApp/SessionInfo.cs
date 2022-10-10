using System.IO;

using binstarjs03.CubeOBJ.Core.Nbt;

namespace binstarjs03.CubeOBJ.WpfApp;

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
