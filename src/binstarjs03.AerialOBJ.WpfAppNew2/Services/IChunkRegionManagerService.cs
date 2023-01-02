using System;

using binstarjs03.AerialOBJ.Core.MinecraftWorldRefactor;
using binstarjs03.AerialOBJ.Core.Primitives;
using binstarjs03.AerialOBJ.WpfApp.Models;

namespace binstarjs03.AerialOBJ.WpfApp.Services;
public delegate void ChunkRegionReadingErrorHandler(Point2Z<int> coords, Exception e);
public interface IChunkRegionManagerService
{
    Point2ZRange<int> VisibleRegionRange { get; }
    int LoadedRegionsCount { get; }
    int PendingRegionsCount { get; }
    Point2Z<int>? WorkedRegion { get; }

    Point2ZRange<int> VisibleChunkRange { get; }
    int LoadedChunksCount { get; }
    int PendingChunksCount { get; }
    int WorkedChunksCount { get; }

    event Action<RegionModel> RegionLoaded;
    event Action<RegionModel> RegionUnloaded;
    event ChunkRegionReadingErrorHandler RegionLoadingError;
    event ChunkRegionReadingErrorHandler ChunkLoadingError;
    event Action<string> PropertyChanged;

    void Update(Point2Z<float> cameraPos, float unitMultiplier, Size<int> screenSize);
    Block? GetBlock(Point2Z<int> blockCoords);
    void OnSavegameOpened();
    void OnSavegameClosed();
}
