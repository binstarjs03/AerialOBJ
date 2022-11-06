using System.Collections.Generic;
using System.IO;

using binstarjs03.AerialOBJ.Core.CoordinateSystem;
using binstarjs03.AerialOBJ.Core.MinecraftWorld;
using binstarjs03.AerialOBJ.Core.NbtNew;

namespace binstarjs03.AerialOBJ.WpfApp;

// TODO we could cache many things here such as list of valid region files etc
public class SavegameLoadInfo
{
    public string WorldName { get; }
    public DirectoryInfo SavegameDirectory { get; }
    public Dictionary<Coords2, FileInfo> RegionFiles { get; }

    public SavegameLoadInfo(DirectoryInfo savegameDirectory, NbtCompound nbtLevel)
    {
        WorldName = nbtLevel.Get<NbtCompound>("Data")
                            .Get<NbtString>("LevelName")
                            .Value;
        SavegameDirectory = savegameDirectory;
        RegionFiles = new Dictionary<Coords2, FileInfo>();

        string savegameDir = savegameDirectory.FullName;
        string regionDirPath = $"{savegameDir}/region";
        DirectoryInfo regionDir = new(regionDirPath);
        FileInfo[] regionFiles = regionDir.GetFiles();
        foreach (FileInfo regionFile in regionFiles)
        {
            string regionFilename = regionFile.Name;
            if (!Region.IsValidFilename(regionFilename, out Coords2? regionCoords))
                continue;
            // region file exceed a single sector of 4KiB (chunk header table),
            // we can assume that is a valid region file
            if (regionFile.Length > Region.SectorDataSize)
                RegionFiles.Add((Coords2)regionCoords!, regionFile);
        }
    }
}
