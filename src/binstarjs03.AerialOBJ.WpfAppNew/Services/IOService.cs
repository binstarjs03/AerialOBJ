﻿/*
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
using binstarjs03.AerialOBJ.WpfAppNew.Models;

namespace binstarjs03.AerialOBJ.WpfAppNew.Services;

public static class IOService
{
    public static void WriteText(string path, string content)
    {
        using StreamWriter writer = File.CreateText(path);
        writer.Write(content);
    }

    public static SavegameLoadInfo? LoadSavegame(string path, out Exception? e)
    {
        e = null;
        DirectoryInfo di = new(path);
        LogService.Log($"Selected path: \"{di.FullName}\"");
        LogService.Log($"Loading \"{di.Name}\" as Minecraft savegame folder...");

        string nbtLevelPath = $"{di.FullName}/level.dat";
        if (!File.Exists(nbtLevelPath))
        {
            string msg = "Missing \"level.dat\" file in specified savegame folder";
            e = new FileNotFoundException(msg);
            return null;
        }
        try
        {
            NbtCompound nbtLevel = (NbtCompound)NbtIO.ReadDisk(nbtLevelPath);
            SavegameLoadInfo ret = new(di, nbtLevel);
            return ret;
        }
        catch (Exception ex)
        {
            e = ex;
            return null;
        }
    }

    public static Dictionary<Coords2, FileInfo> GetRegionFileInfo(string savegameDir)
    {
        Dictionary<Coords2, FileInfo> regionFileInfos = new();

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
                regionFileInfos.Add((Coords2)regionCoords!, regionFile);
        }
        return regionFileInfos;
    }

    public static Region? ReadRegionFile(Coords2 regionCoords, out Exception? e)
    {
        e = null;
        if (StateService.SavegameLoadInfo is null 
            || !StateService.SavegameLoadInfo.RegionFiles.ContainsKey(regionCoords))
            return null;
        try
        {
            // TODO file may be inaccessible already, e.g deliberately deleted by the user,
            // locked by other process, etc
            string regionFilePath = StateService.SavegameLoadInfo.RegionFiles[regionCoords].FullName;
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