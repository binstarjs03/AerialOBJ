using System;
using System.Collections.Generic;
using System.IO;

using binstarjs03.AerialOBJ.Core.Nbt;
using binstarjs03.AerialOBJ.WpfAppNew2.Components;

namespace binstarjs03.AerialOBJ.WpfAppNew2.Services;
public class SavegameLoaderService : ISavegameLoaderService
{
    private readonly GlobalState _globalState;

    public SavegameLoaderService(GlobalState globalState)
    {
        _globalState = globalState;
    }

    // TODO consider moving IO related task to IOService
    public SavegameLoadInfo? LoadSavegame(string savegameDirPath, out Exception? e)
    {
        e = null;
        try
        {
            DirectoryInfo savegameDirInfo = new(savegameDirPath);
            NbtCompound levelNbt = LoadLevelNbt(savegameDirInfo);
            string worldName = GetWorldName(levelNbt, savegameDirPath);
            return new SavegameLoadInfo(worldName, savegameDirInfo);
        }
        catch (Exception ex)
        {
            e = ex;
            return null;
        }

    }

    private static NbtCompound LoadLevelNbt(DirectoryInfo savegameDirInfo)
    {
        string levelNbtPath = $"{savegameDirInfo.FullName}/level.dat";
        if (!File.Exists(levelNbtPath))
        {
            string msg = "Missing \"level.dat\" file in specified savegame folder";
            throw new LevelDatNotFoundException(msg);
        }
        return (NbtCompound)NbtIO.ReadDisk(levelNbtPath);
    }

    private static string GetWorldName(NbtCompound levelNbt, string savegameDirPath)
    {
        try
        {
            return levelNbt.Get<NbtCompound>("Data")
                           .Get<NbtString>("LevelName")
                           .Value;
        }
        // catch NbtNotFoundException
        catch (KeyNotFoundException ex)
        {
            string msg = $"Cannot open \"{savegameDirPath}\" as Minecraft savegame folder, "
                        + "Savegame may be corrupted or not supported "
                        + $"by this version of {GlobalState.AppName}.\n\n"
                        + "Please see the Debug Log Window for more information.";
            throw new NbtNotFoundException(msg, ex);
        }
    }
}
