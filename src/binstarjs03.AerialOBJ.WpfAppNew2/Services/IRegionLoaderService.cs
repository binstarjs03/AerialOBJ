using System;
using System.Threading;

using binstarjs03.AerialOBJ.Core.MinecraftWorldRefactor;
using binstarjs03.AerialOBJ.Core.Primitives;

namespace binstarjs03.AerialOBJ.WpfApp.Services;
public interface IRegionLoaderService
{
    Region? LoadRegion(Point2Z<int> regionCoords, CancellationToken cancellationToken, out Exception? e);
    void PurgeCache();
    void PurgeCache(Point2Z<int> visibleRegionRange, int distance);
}
