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

    public static SessionInfo? LoadSession(string path)
    {
        DirectoryInfo di = new(path);
        LogService.Log($"Selected path: \"{di.FullName}\"");
        LogService.Log($"Loading \"{di.Name}\" as Minecraft savegame folder...");

        string nbtLevelPath = $"{di.FullName}/level.dat";
        if (!File.Exists(nbtLevelPath))
        {
            string msg = "Missing \"level.dat\" file in specified folder";
            LogService.LogError($"{msg}.");
            ShowLoadSavegameErrorModal(path, msg);
            return null;
        }
        try
        {
            NbtCompound nbtLevel = (NbtCompound)NbtIO.ReadDisk(nbtLevelPath);
            SessionInfo ret = new(di, nbtLevel);
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

        static void ShowLoadSavegameErrorModal(string path, string errorMsg)
        {
            string msg = $"Cannot open \"{path}\" as Minecraft savegame folder: \n"
                         + errorMsg;
            ModalService.ShowErrorOK("Error Opening Minecraft Savegame", msg);
        }
    }

    public static bool HasRegionFile(Coords2 regionCoords)
    {
        if (App.CurrentCast.Properties.SessionInfo is null)
            return false;
        string savegameDir = App.CurrentCast.Properties.SessionInfo.SavegameDirectory.FullName;
        string regionPath = $"{savegameDir}/region/r.{regionCoords.X}.{regionCoords.Z}.mca";
        if (File.Exists(regionPath))
        {
            FileInfo fi = new(regionPath);
            // region file exceed a single sector of 4KiB (chunk header table),
            // we can assume that is a valid region file
            if (fi.Length > Region.SectorDataSize)
                return true;
            return false;
        }
        return false;
    }

    public static Region? ReadRegionFile(Coords2 regionCoords, out Exception? e)
    {
        e = null;
        if (App.CurrentCast.Properties.SessionInfo is null)
            return null;
        string savegameDir = App.CurrentCast.Properties.SessionInfo.SavegameDirectory.FullName;
        string regionPath = $"{savegameDir}/region/r.{regionCoords.X}.{regionCoords.Z}.mca";
        if (File.Exists(regionPath))
        {
            try
            {
                Region region = Region.Open(regionPath);
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
