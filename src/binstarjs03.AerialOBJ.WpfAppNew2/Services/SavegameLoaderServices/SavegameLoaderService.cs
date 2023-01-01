using System;
using System.Collections.Generic;
using System.IO;

using binstarjs03.AerialOBJ.Core.Nbt;
using binstarjs03.AerialOBJ.WpfAppNew2.Components;

namespace binstarjs03.AerialOBJ.WpfAppNew2.Services.SavegameLoaderServices;
public class SavegameLoaderService : ISavegameLoaderService
{
    public SavegameLoadInfo LoadSavegame(string savegameDirPath)
    {
        if (!Directory.Exists(savegameDirPath))
            throw new DirectoryNotFoundException($"Directory to {savegameDirPath} not found");
        DirectoryInfo savegameDirInfo = new(savegameDirPath);
        NbtCompound levelNbt = LoadLevelNbt(savegameDirInfo);
        return new SavegameLoadInfo()
        {
            SavegameDirectoryPath = savegameDirInfo.FullName,
            DataVersion = GetDataVersion(levelNbt),
            WorldName = GetWorldName(levelNbt),
        };
    }

    private static NbtCompound LoadLevelNbt(DirectoryInfo savegameDirInfo)
    {
        string errorHeader = "Cannot read savegame:\n";
        string levelNbtPath = $"{savegameDirInfo.FullName}/level.dat";
        if (!File.Exists(levelNbtPath))
        {
            string msg = $"{errorHeader}Missing level.dat file in specified savegame folder";
            throw new LevelDatNotFoundException(msg);
        }
        FileInfo levelDatInfo = new(levelNbtPath);
        if (levelDatInfo.Length == 0)
            throw new InvalidDataException($"{errorHeader}Invalid level.dat file. No data exist");
        try
        {
            return (NbtCompound)NbtIO.ReadDisk(levelNbtPath);
        }
        catch (NbtUnknownCompressionMethodException e)
        {
            throw new LevelDatUnreadableException($"{errorHeader}Unknown compression method for level.dat file", e);
        }
        catch (Exception e)
        {
            if (e is InvalidDataException || e is NbtException || e is EndOfStreamException)
                throw new LevelDatUnreadableException($"{errorHeader}level.dat file is corrupted:\n{e.Message}", e);
            // anything else that is not above exceptions is unhandled,
            // and we don't want to display cryptic message to the user
            else if (e is UnauthorizedAccessException)
                throw new UnauthorizedAccessException($"{errorHeader}{GlobalState.AppName} do not have the permission to access level.dat file", e);
            else
                throw new Exception($"{errorHeader}Unhandled exception while deserializing level.dat file:\n{e.Message}", e);
        }
    }

    private int GetDataVersion(NbtCompound levelNbt)
    {
        throw new NotImplementedException();
    }

    private static string GetWorldName(NbtCompound levelNbt)
    {
        try
        {
            return levelNbt.Get<NbtCompound>("Data")
                           .Get<NbtString>("LevelName")
                           .Value;
        }
        catch (KeyNotFoundException ex)
        {
            string msg = "Mismatch NBT structure of level.dat file. "
                       + $"Savegame is not supported by this version of {GlobalState.AppName}.";
            throw new NbtException(msg, ex);
        }
    }
}
