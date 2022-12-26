using System;

using binstarjs03.AerialOBJ.Core.MinecraftWorld;
using binstarjs03.AerialOBJ.Core.Primitives;

namespace binstarjs03.AerialOBJ.WpfAppNew2.Services;
public delegate void RegionReadingErrorHandler(Point2Z<int> regionCoords, Exception e);
public interface IChunkRegionManagerService
{
    Point2ZRange<int> VisibleRegionRange { get; }
    int LoadedRegionsCount { get; }
    int PendingRegionsCount { get; }
    Point2Z<int>? WorkedRegion { get; }
    Point2ZRange<int> VisibleChunkRange { get; }

    void Update(Point2Z<float> cameraPos, float unitMultiplier, Size<int> screenSize);

    event Action<Region> RegionLoaded;
    event Action<Region> RegionUnloaded;
    event RegionReadingErrorHandler RegionReadingError;
    event Action<string> PropertyChanged;
}
