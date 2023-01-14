using binstarjs03.AerialOBJ.WpfApp.Components;

namespace binstarjs03.AerialOBJ.WpfApp.Services.IOService.SavegameLoaderServices;
public interface ISavegameLoaderService
{
    SavegameLoadInfo LoadSavegame(string savegameDirPath);
}