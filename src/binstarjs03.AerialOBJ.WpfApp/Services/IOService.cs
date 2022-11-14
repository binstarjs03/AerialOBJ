/*
Copyright (c) 2022, Bintang Jakasurya
All rights reserved. 

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System;
using System.IO;

using binstarjs03.AerialOBJ.Core.CoordinateSystem;
using binstarjs03.AerialOBJ.Core.MinecraftWorld;
using binstarjs03.AerialOBJ.Core.Nbt;

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
                            + $"by this version of {AppState.AppName}.";
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
