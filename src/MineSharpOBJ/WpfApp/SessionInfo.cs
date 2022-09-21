using System.IO;
using binstarjs03.MineSharpOBJ.Core.Nbt.Concrete;
namespace binstarjs03.MineSharpOBJ.WpfApp;

public struct SessionInfo
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
