using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

using binstarjs03.AerialOBJ.Core.MinecraftWorld;
using binstarjs03.AerialOBJ.Core.Primitives;
using binstarjs03.AerialOBJ.WpfApp.Components;

namespace binstarjs03.AerialOBJ.WpfApp.Services;
public class RegionDiskLoader : IRegionDiskLoader
{
    private readonly GlobalState _globalState;
    private readonly Dictionary<Point2Z<int>, Region> _cachedRegions = new(100);

    public RegionDiskLoader(GlobalState globalState)
    {
        _globalState = globalState;
    }

    public Region? LoadRegion(Point2Z<int> regionCoords, CancellationToken cancellationToken)
    {
        if (_cachedRegions.TryGetValue(regionCoords, out Region? region))
            return region;

        string regionPath = getRegionPath(regionCoords);
        if (!File.Exists(regionPath))
            return null;
        byte[] regionData = File.ReadAllBytes(regionPath);
        region = new(regionData, regionCoords);
        _cachedRegions.Add(regionCoords, region);
        return region;

        string getRegionPath(Point2Z<int> regionCoords)
        {
            SavegameLoadInfo? loadInfo = App.Current.Dispatcher.Invoke(() => _globalState.SavegameLoadInfo, DispatcherPriority.Normal, cancellationToken);
            if (loadInfo is null)
                throw new TaskCanceledException();
            return Path.Combine(loadInfo.SavegameDirectoryPath, "region", $"r.{regionCoords.X}.{regionCoords.Z}.mca");
        }
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