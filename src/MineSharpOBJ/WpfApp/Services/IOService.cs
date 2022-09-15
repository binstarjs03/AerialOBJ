using System;
using System.IO;
namespace binstarjs03.MineSharpOBJ.WpfApp.Services;

public class IOService {
    public static DirectoryInfo[] GetSavegamePaths() {
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
}
