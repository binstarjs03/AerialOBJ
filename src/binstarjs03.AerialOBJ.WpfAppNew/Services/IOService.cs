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
using System.Collections.Generic;
using System.IO;

using binstarjs03.AerialOBJ.Core.CoordinateSystem;
using binstarjs03.AerialOBJ.Core.MinecraftWorld;
using binstarjs03.AerialOBJ.Core.Nbt;
using binstarjs03.AerialOBJ.WpfAppNew.Components;

namespace binstarjs03.AerialOBJ.WpfAppNew.Services;

public static class IOService
{
    public static void WriteText(string path, string content)
    {
        using StreamWriter writer = File.CreateText(path);
        writer.Write(content);
    }

    /// <exception cref="LevelDatNotFoundException"></exception>
    public static SavegameLoadInfo LoadSavegame(string path, out bool foundRegionFolder)
    {
        DirectoryInfo di = new(path);
        string nbtLevelPath = $"{di.FullName}/level.dat";
        if (!File.Exists(nbtLevelPath))
            throw new LevelDatNotFoundException();
        NbtCompound nbtLevel = (NbtCompound)NbtIO.ReadDisk(nbtLevelPath);
        return SavegameLoadInfo.LoadSavegame(di, nbtLevel, out foundRegionFolder);
    }

    /// <exception cref="RegionFolderNotFoundException"></exception>
    public static Dictionary<Coords2, FileInfo> GetRegionFileInfo(string savegameDir)
    {
        Dictionary<Coords2, FileInfo> regionFileInfos = new();

        string regionDirPath = $"{savegameDir}/region";
        DirectoryInfo regionDir = new(regionDirPath);
        FileInfo[] regionFiles;
        try
        {
            regionFiles = regionDir.GetFiles();
        }
        catch (DirectoryNotFoundException e)
        {
            string msg = $"Folder \"region\" does not exist in {savegameDir}";
            throw new RegionFolderNotFoundException(msg, e);
        }
        foreach (FileInfo regionFile in regionFiles)
        {
            string regionFilename = regionFile.Name;
            if (!Region.IsValidFilename(regionFilename, out Coords2? regionCoords))
                continue;
            // region file exceed a single sector of 4KiB (chunk header table),
            // we can assume that is a valid region file
            if (regionFile.Length > Region.SectorDataSize)
                regionFileInfos.Add((Coords2)regionCoords!, regionFile);
        }
        return regionFileInfos;
    }

    // TODO maybe we should disable caching region file existence
    public static Region? ReadRegionFile(Coords2 regionCoords, out Exception? e)
    {
        e = null;
        if (SharedStateService.SavegameLoadInfo is null
            || SharedStateService.SavegameLoadInfo.RegionFiles is null
            || !SharedStateService.SavegameLoadInfo.RegionFiles.ContainsKey(regionCoords))
            return null;
        try
        {
            // TODO file may be inaccessible already, e.g deliberately deleted by the user,
            // locked by other process, etc
            string regionFilePath = SharedStateService.SavegameLoadInfo.RegionFiles[regionCoords].FullName;
            Region region = new(regionFilePath, regionCoords);
            return region;
        }
        catch (Exception ex)
        {
            e = ex;
            return null;
        }
    }
}
