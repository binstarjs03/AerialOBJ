using System.Collections.Generic;

using binstarjs03.AerialOBJ.Core.Primitives;
using binstarjs03.AerialOBJ.Core.MinecraftWorld;

namespace binstarjs03.AerialOBJ.WpfAppNew.Services;
public static class RegionCacheService
{
    private static readonly Dictionary<Point2Z<int>, Region> s_regionCache = new();

    public static bool HasRegion(Point2Z<int> regionCoords)
    {
        return s_regionCache.ContainsKey(regionCoords);
    }

    public static Region Get(Point2Z<int> regionCoords)
    {
        return s_regionCache[regionCoords];
    }

    public static void Store(Region region)
    {
        if (HasRegion(region.RegionCoords))
            return;
        s_regionCache.Add(region.RegionCoords, region);
    }

    public static void Delete(Point2Z<int> regionCoords)
    {
        if (HasRegion(regionCoords))
            s_regionCache.Remove(regionCoords);
    }

    public static void Clear()
    {
        s_regionCache.Clear();
    }
}
