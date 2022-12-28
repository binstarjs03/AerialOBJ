using System;

using binstarjs03.AerialOBJ.Core.Primitives;
using binstarjs03.AerialOBJ.WpfAppNew2.Models;

namespace binstarjs03.AerialOBJ.WpfAppNew2.Services;
public delegate void RegionReadingErrorHandler(Point2Z<int> regionCoords, Exception e);
public interface IChunkRegionManagerService
{
    Point2ZRange<int> VisibleRegionRange { get; }
    int LoadedRegionsCount { get; }
    int PendingRegionsCount { get; }
    Point2Z<int>? WorkedRegion { get; }
    Point2ZRange<int> VisibleChunkRange { get; }

    event Action<RegionModel> RegionImageLoaded;
    event Action<RegionModel> RegionImageUnloaded;
    event RegionReadingErrorHandler RegionLoadingError;
    event Action<string> PropertyChanged;

    void Update(Point2Z<float> cameraPos, float unitMultiplier, Size<int> screenSize);
    void Reinitialize();
}
