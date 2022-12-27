using System;

using binstarjs03.AerialOBJ.Core.MinecraftWorld;
using binstarjs03.AerialOBJ.Core.Primitives;

namespace binstarjs03.AerialOBJ.WpfAppNew2.Services;
public class RegionLoaderService : IRegionLoaderService
{
    private readonly IIOService _iOService;

    public RegionLoaderService(IIOService iOService)
    {
        _iOService = iOService;
    }

    public Region? LoadRegion(Point2Z<int> regionCoords, out Exception? e)
    {
        return _iOService.ReadRegionFile(regionCoords, out e);
    }
}