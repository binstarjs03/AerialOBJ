﻿using System;

using binstarjs03.AerialOBJ.Core.Primitives;
using binstarjs03.AerialOBJ.WpfAppNew2.Models;

namespace binstarjs03.AerialOBJ.WpfAppNew2.Services;
public delegate void RegionReadingErrorHandler(Point2Z<int> regionCoords, Exception e);
public interface IChunkRegionManagerService
{
    Point2ZRange<int> VisibleChunkRange { get; }
    int LoadedRegionsCount { get; }
    int PendingRegionsCount { get; }
    Point2Z<int>? WorkedRegion { get; }
    bool NoWorkedRegion { get; }

    Point2ZRange<int> VisibleRegionRange { get; }

    void Update(Point2Z<float> cameraPos, float unitMultiplier, Size<int> screenSize);

    event Action<RegionImageModel> RegionImageAdded;
    event Action<RegionImageModel> RegionImageRemoved;
    event RegionReadingErrorHandler RegionReadingError;
    event Action<string> PropertyChanged;
}
