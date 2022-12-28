using System;
using System.Collections.Generic;

using binstarjs03.AerialOBJ.Core.MinecraftWorld;
using binstarjs03.AerialOBJ.Core.Primitives;

namespace binstarjs03.AerialOBJ.WpfAppNew2.Services;
public class RegionLoaderService : IRegionLoaderService
{
    private readonly IIOService _iOService;
    private readonly Dictionary<Point2Z<int>, Region> _cachedRegions = new(100);

    public RegionLoaderService(IIOService iOService)
    {
        _iOService = iOService;
    }

    public Region? LoadRegion(Point2Z<int> regionCoords, out Exception? e)
    {
        e = null;
        // try to get region from cache, else read it from IOService
        if (_cachedRegions.TryGetValue(regionCoords, out Region? region))
            return region;

        Region? loadResult = _iOService.ReadRegionFile(regionCoords, out e);
        if (loadResult is not null)
        {
            _cachedRegions.Add(regionCoords, loadResult);
            return loadResult;
        }

        return null;
    }

    public void PurgeCache()
    {
        _cachedRegions.Clear();
    }

    public void PurgeCache(Point2Z<int> visibleRegionRange, int distance)
    {
        throw new NotImplementedException();
    }
}