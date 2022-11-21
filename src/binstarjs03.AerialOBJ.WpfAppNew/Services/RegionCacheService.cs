using System.Collections.Generic;

using binstarjs03.AerialOBJ.Core.CoordinateSystem;
using binstarjs03.AerialOBJ.Core.MinecraftWorld;

namespace binstarjs03.AerialOBJ.WpfAppNew.Services;
public static class RegionCacheService
{
    private static Dictionary<Coords2, Region> _regionCache = new();

    public static bool HasRegion(Coords2 regionCoords)
    {
        return _regionCache.ContainsKey(regionCoords);
    }

    public static void Store(Region region)
    {
        if (HasRegion(region.RegionCoords))
            return;
        _regionCache.Add(region.RegionCoords, region);
    }

    public static void Delete(Coords2 regionCoords)
    {
        if (HasRegion(regionCoords))
            _regionCache.Remove(regionCoords);
    }

    public static void Clear()
    {
        _regionCache.Clear();
    }
}
