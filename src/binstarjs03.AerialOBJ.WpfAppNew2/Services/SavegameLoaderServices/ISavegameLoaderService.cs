using binstarjs03.AerialOBJ.WpfAppNew2.Components;

namespace binstarjs03.AerialOBJ.WpfAppNew2.Services.SavegameLoaderServices;
public interface ISavegameLoaderService
{
    SavegameLoadInfo LoadSavegame(string savegameDirPath);
}