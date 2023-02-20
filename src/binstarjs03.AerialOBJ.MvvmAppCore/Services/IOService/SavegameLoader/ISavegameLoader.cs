using binstarjs03.AerialOBJ.MvvmAppCore.Models;

namespace binstarjs03.AerialOBJ.MvvmAppCore.Services.IOService.SavegameLoader;
public interface ISavegameLoader
{
    SavegameLoadInfo LoadSavegame(string savegameDirPath);
}