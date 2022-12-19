using System;

using binstarjs03.AerialOBJ.WpfAppNew2.Components;

namespace binstarjs03.AerialOBJ.WpfAppNew2.Services;
public interface ISavegameLoaderService
{
    SavegameLoadInfo? LoadSavegame(string savegameDirPath);
}