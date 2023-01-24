using System;
using System.IO;

using binstarjs03.AerialOBJ.Core.Nbt;
using binstarjs03.AerialOBJ.WpfApp.Models;

namespace binstarjs03.AerialOBJ.WpfApp.Services.IOService.SavegameLoader;
public class SavegameLoader : ISavegameLoader
{
    public SavegameLoadInfo LoadSavegame(string savegameDirPath)
    {
        DirectoryInfo savegameDirInfo = new(savegameDirPath);
        NbtCompound levelNbt = LoadLevelNbt(savegameDirInfo);
        return ParseLevelNbtStructure(levelNbt, savegameDirInfo.FullName);
    }

    // TODO refactor this method
    private static NbtCompound LoadLevelNbt(DirectoryInfo savegameDirInfo)
    {
        string errorHeader = "Cannot read savegame:\n";
        string levelNbtPath = Path.Combine(savegameDirInfo.FullName, "level.dat");
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

    // TODO refactor this to conform open-closed principle,
    // maybe use separate class for dataversion parser,
    // put it in a collection and iterate over which dataversion parser range it supports
    // TODO we may create DataVersion class that contains what IChunk implementation it use, Level.dat parser, etc
    private static SavegameLoadInfo ParseLevelNbtStructure(NbtCompound levelNbt, string savegameDirectoryPath)
    {
        int dataVersion = GetDataVersion(levelNbt);
        try
        {
            if (dataVersion >= 2860)
                return ParseDataVersion2860(levelNbt, savegameDirectoryPath);
            throw new LevelDatNotImplementedParserException($"No parser found for dataversion {dataVersion}");
        }
        catch (Exception e)
        {
            string unsupportedMsg = "Mismatch NBT structure of level.dat file. " +
                                   $"Savegame is not supported by this version of {GlobalState.AppName}.";
            throw new NbtException(unsupportedMsg, e);
        }
    }

    private static int GetDataVersion(NbtCompound levelNbt)
    {
        return levelNbt.Get<NbtCompound>("Data")
                       .Get<NbtInt>("DataVersion").Value;
    }

    private static SavegameLoadInfo ParseDataVersion2860(NbtCompound levelNbt, string savegameDirectoryPath)
    {
        return new SavegameLoadInfo()
        {
            DataVersion = 2860,
            WorldName = getWorldName(),
            SavegameDirectoryPath = savegameDirectoryPath,
            HighHeightLimit = 319,
            LowHeightLimit = -64
        };

        string getWorldName()
        {
            return levelNbt.Get<NbtCompound>("Data")
                           .Get<NbtString>("LevelName")
                           .Value;
        }
    }
}
