using binstarjs03.AerialOBJ.WpfApp.Components;

namespace binstarjs03.AerialOBJ.WpfApp.Services.IOService.SavegameLoader;
public interface ISavegameLoader
{
    SavegameLoadInfo LoadSavegame(string savegameDirPath);
}