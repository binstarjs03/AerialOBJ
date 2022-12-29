using System.Collections.Generic;

using binstarjs03.AerialOBJ.Core.Primitives;

namespace binstarjs03.AerialOBJ.WpfAppNew2.Components;
public class ChunkRegionManagerErrorMemory : IChunkRegionManagerErrorMemory
{
    private readonly HashSet<Point2Z<int>> _regionErrors = new();
    private readonly HashSet<Point2Z<int>> _chunkErrors = new();

    public void StoreRegionError(Point2Z<int> regionCoords)
    {
        _regionErrors.Add(regionCoords);
    }

    public void StoreChunkError(Point2Z<int> chunkCoords)
    {
        _chunkErrors.Add(chunkCoords);
    }

    public bool CheckHasRegionError(Point2Z<int> regionCoords)
    {
        return _regionErrors.Contains(regionCoords);
    }

    public bool CheckHasChunkError(Point2Z<int> chunkCoords)
    {
        return _chunkErrors.Contains(chunkCoords);
    }

    public void Reinitialize()
    {
        _regionErrors.Clear();
        _chunkErrors.Clear();
    }
}
