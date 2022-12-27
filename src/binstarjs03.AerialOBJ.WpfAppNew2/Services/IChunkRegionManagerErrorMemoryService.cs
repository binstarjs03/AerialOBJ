using binstarjs03.AerialOBJ.Core.Primitives;

namespace binstarjs03.AerialOBJ.WpfAppNew2.Services;
public interface IChunkRegionManagerErrorMemoryService
{
    bool CheckHasChunkError(Point2Z<int> chunkCoords);
    bool CheckHasRegionError(Point2Z<int> regionCoords);
    void Reinitialize();
    void StoreChunkError(Point2Z<int> chunkCoords);
    void StoreRegionError(Point2Z<int> regionCoords);
}