using System;

using binstarjs03.AerialOBJ.Core.MinecraftWorld;
using binstarjs03.AerialOBJ.Core.Primitives;
using binstarjs03.AerialOBJ.WpfApp.Models;

namespace binstarjs03.AerialOBJ.WpfApp.Services;
public delegate void ChunkRegionReadingErrorHandler(PointZ<int> coords, Exception e);
public interface IChunkRegionManager
{
    PointZRange<int> VisibleRegionRange { get; }
    int VisibleRegionsCount { get; }
    int LoadedRegionsCount { get; }
    int CachedRegionsCount { get; }
    int PendingRegionsCount { get; }
    PointZ<int>? WorkedRegion { get; }

    PointZRange<int> VisibleChunkRange { get; }
    int VisibleChunksCount { get; }
    int LoadedChunksCount { get; }
    int PendingChunksCount { get; }
    int WorkedChunksCount { get; }

    event Action<RegionDataImageModel> RegionLoaded;
    event Action<RegionDataImageModel> RegionUnloaded;
    event ChunkRegionReadingErrorHandler RegionLoadingException;
    event ChunkRegionReadingErrorHandler ChunkLoadingException;

    void Update(PointZ<float> cameraPos, float unitMultiplier, Size<int> screenSize);
    void UpdateHeightLevel(int heightLevel);
    void ReloadRenderedChunks();
    BlockSlim? GetHighestBlockAt(PointZ<int> blockCoords);
    void StartBackgroundThread();
    void StopBackgroundThread();
    void Reinitialize();
}
