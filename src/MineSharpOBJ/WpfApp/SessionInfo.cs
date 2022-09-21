using System.IO;

using NbtCompound = binstarjs03.MineSharpOBJ.Core.Nbt.Concrete.NbtCompound;
using NbtString = binstarjs03.MineSharpOBJ.Core.Nbt.Concrete.NbtString;

namespace binstarjs03.MineSharpOBJ.WpfApp;

public class SessionInfo {
    public SessionInfo(DirectoryInfo savegameDirectory, NbtCompound nbtLevel) {
        WorldName = nbtLevel.Get<NbtCompound>("Data")
                            .Get<NbtString>("LevelName")
                            .Value;
        SavegameDirectory = savegameDirectory;
    }

    public string WorldName { get; }

    public DirectoryInfo SavegameDirectory { get; }
}
