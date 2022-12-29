using System;

using binstarjs03.AerialOBJ.Core.MinecraftWorld;
using binstarjs03.AerialOBJ.Core.Primitives;
using binstarjs03.AerialOBJ.WpfAppNew2.Components;

namespace binstarjs03.AerialOBJ.WpfAppNew2.Services;
public interface IIOService
{
    bool WriteText(string path, string content, out Exception? e);

    Region? ReadRegionFile(Point2Z<int> regionCoords, SavegameLoadInfo loadInfo, out Exception? e);
}
