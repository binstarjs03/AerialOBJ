using System;
using System.IO;

using binstarjs03.AerialOBJ.Core.CoordinateSystem;
using binstarjs03.AerialOBJ.Core.NbtNew;
using binstarjs03.AerialOBJ.Core.MinecraftWorld;

namespace binstarjs03.AerialOBJ.WpfApp.Services;

public static class IOService
{
    public static DirectoryInfo[] GetSavegameDirectories()
    {
        string userPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        string minecraftSaveRootPath = $"{userPath}/AppData/Roaming/.minecraft/saves";
        if (!Directory.Exists(minecraftSaveRootPath))
        {
            throw new DirectoryNotFoundException("Minecraft savefolders not found");
        }
        DirectoryInfo di = new(minecraftSaveRootPath);
        DirectoryInfo[] savePaths = di.GetDirectories();
        return savePaths;
    }

    /// <exception cref="IOException"/>
    /// <exception cref="PathTooLongException"/>
    /// <exception cref="DirectoryNotFoundException"/>
    /// <exception cref="UnauthorizedAccessException"/>
    public static void WriteText(string path, string content)
    {
        using StreamWriter writer = File.CreateText(path);
        writer.Write(content);
    }

    public static SavegameLoadInfo? LoadSavegame(string path)
    {
        DirectoryInfo di = new(path);
        LogService.Log($"Selected path: \"{di.FullName}\"");
        LogService.Log($"Loading \"{di.Name}\" as Minecraft savegame folder...");

        string nbtLevelPath = $"{di.FullName}/level.dat";
        if (!File.Exists(nbtLevelPath))
        {
            string msg = "Missing \"level.dat\" file in specified savegame folder";
            LogService.LogError($"{msg}.");
            ShowLoadSavegameErrorModal(path, msg);
            return null;
        }
        try
        {
            LogService.Log("Found \"level.dat\" file, reading NBT data...");
            NbtCompound nbtLevel = (NbtCompound)NbtIO.ReadDisk(nbtLevelPath);
            LogService.Log("Successfully parsed \"level.dat\" NBT data, savegame folder is valid");
            SavegameLoadInfo ret = new(di, nbtLevel);
            LogService.Log($"Successfully loaded \"{di.Name}\" (\"{ret.WorldName}\")");
            return ret;
        }
        catch (Exception ex)
        {
            string modalMsg = "Failed when reading \"level.dat\" file "
                            + "in specified folder, "
                            + "may be corrupted or not supported "
                            + "by this version of MineSharpOBJ.";
            string logMsg = $"{modalMsg}\n\n"
                          + "Exception details:\n"
                          + $"{ex.GetType()}: {ex.Message}";
            LogService.LogError(logMsg);
            ShowLoadSavegameErrorModal(path, modalMsg);
            return null;
        }

        // TODO i guess services should not interact with UI elemets such as modal,
        // maybe we should leave that job to the caller and its up to them
        static void ShowLoadSavegameErrorModal(string path, string errorMsg)
        {
            string msg = $"Cannot open \"{path}\" as Minecraft savegame folder: \n"
                         + errorMsg;
            ModalService.ShowErrorOK("Error Opening Minecraft Savegame", msg);
        }
    }

    public static bool HasRegionFile(Coords2 regionCoords)
    {
        if (App.Current.State.SavegameLoadInfo is null)
            return false;
        else if (App.Current.State.SavegameLoadInfo.RegionFiles.ContainsKey(regionCoords))
            return true;
        else
            return false;
    }

    public static Region? ReadRegionFile(Coords2 regionCoords, out Exception? e)
    {
        e = null;
        if (App.Current.State.SavegameLoadInfo is null)
            return null;
        if (HasRegionFile(regionCoords))
        {
            try
            {
                string regionFilePath = App.Current.State.SavegameLoadInfo.RegionFiles[regionCoords].FullName;
                Region region = new(regionFilePath, regionCoords);
                return region;
            }
            catch (Exception ex)
            {
                e = ex;
                return null;
            }
        }
        return null;
    }
}
