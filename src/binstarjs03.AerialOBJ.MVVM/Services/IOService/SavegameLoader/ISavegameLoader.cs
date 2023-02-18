using binstarjs03.AerialOBJ.MVVM.Models;

namespace binstarjs03.AerialOBJ.MVVM.Services.IOService.SavegameLoader;
public interface ISavegameLoader
{
    SavegameLoadInfo LoadSavegame(string savegameDirPath);
}