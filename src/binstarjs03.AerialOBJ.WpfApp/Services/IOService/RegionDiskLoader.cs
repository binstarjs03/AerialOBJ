using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;

using binstarjs03.AerialOBJ.Core.MinecraftWorld;
using binstarjs03.AerialOBJ.Core.Primitives;
using binstarjs03.AerialOBJ.WpfApp.Components;
using binstarjs03.AerialOBJ.WpfApp.Services.Dispatcher;

namespace binstarjs03.AerialOBJ.WpfApp.Services.IOService;
public class RegionDiskLoader : IRegionDiskLoader
{
    private readonly GlobalState _globalState;
    private readonly IDispatcher _dispatcher;

    public RegionDiskLoader(GlobalState globalState, IDispatcher dispatcher)
    {
        _globalState = globalState;
        _dispatcher = dispatcher;
    }

    public bool TryGetRegion(Point2Z<int> regionCoords, CancellationToken ct, [NotNullWhen(true)] out IRegion? region)
    {
        region = null;
        string? regionPath = getRegionPath(regionCoords);
        if (regionPath is null)
            return false;

        // exception is expensive, we don't want to throw because we are
        // frequently accessing files that may exist or not, and we can
        // check if file exist beforehand
        if (!File.Exists(regionPath))
            return false;

        // some region files have intentionally zero-length data
        // and make sure to check that before instantiating Region
        byte[] regionData = File.ReadAllBytes(regionPath);
        if (regionData.Length == 0)
            return false;

        region = new Region(regionData, regionCoords);
        return true;

        string? getRegionPath(Point2Z<int> regionCoords)
        {
            SavegameLoadInfo? loadInfo = _dispatcher.Invoke(() => _globalState.SavegameLoadInfo, DispatcherPriority.Normal, ct);
            if (loadInfo is null)
                return null;
            return Path.Combine(loadInfo.SavegameDirectoryPath, "region", $"r.{regionCoords.X}.{regionCoords.Z}.mca");
        }
    }
}
