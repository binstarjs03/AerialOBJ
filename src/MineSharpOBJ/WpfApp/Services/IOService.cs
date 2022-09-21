using System;
using System.IO;
using System.Windows;
using NbtBase = binstarjs03.MineSharpOBJ.Core.Nbt.Abstract.NbtBase;
using NbtCompound = binstarjs03.MineSharpOBJ.Core.Nbt.Concrete.NbtCompound;
using ByteOrder = binstarjs03.MineSharpOBJ.Core.Utils.IO.ByteOrder;
namespace binstarjs03.MineSharpOBJ.WpfApp.Services;

public class IOService {
    public static DirectoryInfo[] GetSavegameDirectories() {
        string userPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        string minecraftSaveRootPath = $"{userPath}/AppData/Roaming/.minecraft/saves";
        if (!Directory.Exists(minecraftSaveRootPath)) {
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
    public static void WriteText(string path, string content) {
        using FileStream stream = File.Open(path,
                                            FileMode.Create,
                                            FileAccess.Write,
                                            FileShare.None);
        using StreamWriter writer = new(stream);
        writer.Write(content);
    }

    public static SessionInfo? LoadSavegame(string path) {
        DirectoryInfo di = new(path);
        LogService.Log($"Selected path: \"{di.FullName}\"");
        LogService.Log($"Loading \"{di.Name}\" as Minecraft savegame folder...");

        string nbtLevelPath = $"{di.FullName}/level.dat";
        if (!File.Exists(nbtLevelPath)) {
            string msg = "Missing \"level.dat\" file in specified folder";
            LogService.LogError($"{msg}.");
            ShowLoadSavegameErrorModal(path, msg);
            return null;
        }
        try {
            NbtCompound nbtLevel = (NbtCompound)NbtBase.ReadDisk(nbtLevelPath, ByteOrder.BigEndian);
            SessionInfo ret = new(di, nbtLevel);
            LogService.Log($"Successfully loaded \"{di.Name}\" (\"{ret.WorldName}\")");
            return ret;
        }
        catch (Exception ex) {
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

        static void ShowLoadSavegameErrorModal(string path, string errorMsg) {
            string msg = $"Cannot open \"{path}\" as Minecraft savegame folder: \n"
                         + errorMsg;
            MessageBox.Show(
                msg,
                caption: "Error Opening Minecraft Savegame",
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );
        }
    }
}
