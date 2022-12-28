using System;

using binstarjs03.AerialOBJ.Core.MinecraftWorld;
using binstarjs03.AerialOBJ.Core.Primitives;

namespace binstarjs03.AerialOBJ.WpfAppNew2.Services;
public interface IRegionLoaderService
{
    Region? LoadRegion(Point2Z<int> regionCoords, out Exception? e);
    void PurgeCache();
    void PurgeCache(Point2Z<int> visibleRegionRange, int distance);
}
