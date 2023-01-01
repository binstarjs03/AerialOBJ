using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

using binstarjs03.AerialOBJ.Core.MinecraftWorldRefactor;
using binstarjs03.AerialOBJ.Core.Primitives;
using binstarjs03.AerialOBJ.WpfAppNew2.Components;

namespace binstarjs03.AerialOBJ.WpfAppNew2.Services;
public class RegionLoaderService : IRegionLoaderService
{
    private readonly IIOService _iOService;
    private readonly GlobalState _globalState;
    private readonly Dictionary<Point2Z<int>, Region> _cachedRegions = new(100);

    public RegionLoaderService(IIOService iOService, GlobalState globalState)
    {
        _iOService = iOService;
        _globalState = globalState;
    }

    public Region? LoadRegion(Point2Z<int> regionCoords, CancellationToken cancellationToken, out Exception? e)
    {
        e = null;
        // try to get region from cache, else read it from IOService
        if (_cachedRegions.TryGetValue(regionCoords, out Region? region))
            return region;

        SavegameLoadInfo? loadInfo;
        try
        {
            // TODO Inconsistency with MutableImageFactory, this one is returning null but RegionLoaderService is throwing
            loadInfo = App.Current.Dispatcher.Invoke(() => _globalState.SavegameLoadInfo, DispatcherPriority.Send, cancellationToken);
            if (loadInfo is null)
                return null;
        }
        catch (TaskCanceledException) { return null; }
        Region? loadResult = _iOService.ReadRegionFile(regionCoords, loadInfo, out e);
        if (loadResult is null)
            return null;
        _cachedRegions.Add(regionCoords, loadResult);
        return loadResult;
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