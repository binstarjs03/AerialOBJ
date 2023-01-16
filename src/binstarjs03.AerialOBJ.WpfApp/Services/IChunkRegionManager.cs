using System;

using binstarjs03.AerialOBJ.Core.MinecraftWorld;
using binstarjs03.AerialOBJ.Core.Primitives;
using binstarjs03.AerialOBJ.WpfApp.Models;

namespace binstarjs03.AerialOBJ.WpfApp.Services;
public delegate void ChunkRegionReadingErrorHandler(Point2Z<int> coords, Exception e);
public interface IChunkRegionManager
{
    Point2ZRange<int> VisibleRegionRange { get; }
    int VisibleRegionsCount { get; }
    int LoadedRegionsCount { get; }
    int CachedRegionsCount { get; }
    int PendingRegionsCount { get; }
    Point2Z<int>? WorkedRegion { get; }

    Point2ZRange<int> VisibleChunkRange { get; }
    int VisibleChunksCount { get; }
    int LoadedChunksCount { get; }
    int PendingChunksCount { get; }
    int WorkedChunksCount { get; }

    event Action<RegionDataImageModel> RegionLoaded;
    event Action<RegionDataImageModel> RegionUnloaded;
    event ChunkRegionReadingErrorHandler RegionLoadingException;
    event ChunkRegionReadingErrorHandler ChunkLoadingException;

    void Update(Point2Z<float> cameraPos, float unitMultiplier, Size<int> screenSize);
    void UpdateHeightLevel(int heightLevel, HeightSliderSetting setting);
    Block? GetHighestBlockAt(Point2Z<int> blockCoords);
    void StartBackgroundThread();
    void StopBackgroundThread();
    void Reinitialize();
}
