using System;

using binstarjs03.AerialOBJ.Core.MinecraftWorldRefactor;
using binstarjs03.AerialOBJ.Core.Primitives;
using binstarjs03.AerialOBJ.WpfApp.Components;

namespace binstarjs03.AerialOBJ.WpfApp.Services;
public interface IIOService
{
    void WriteText(string path, string content);
    Region? ReadRegionFile(Point2Z<int> regionCoords, SavegameLoadInfo loadInfo, out Exception? e);
}
