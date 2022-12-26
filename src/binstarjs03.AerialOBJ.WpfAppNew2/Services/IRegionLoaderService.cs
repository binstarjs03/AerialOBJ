using System;

using binstarjs03.AerialOBJ.Core.MinecraftWorld;
using binstarjs03.AerialOBJ.Core.Primitives;

namespace binstarjs03.AerialOBJ.WpfAppNew2.Services;
public interface IRegionLoaderService
{
    Region? LoadRegion(Point2Z<int> regionCoords, out Exception? e);
}
