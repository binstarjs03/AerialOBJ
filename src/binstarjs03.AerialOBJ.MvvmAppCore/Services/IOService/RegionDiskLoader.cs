using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;

using binstarjs03.AerialOBJ.Core.MinecraftWorld;
using binstarjs03.AerialOBJ.Core.Primitives;
using binstarjs03.AerialOBJ.MvvmAppCore.Models;
using binstarjs03.AerialOBJ.MvvmAppCore.Services.Dispatcher;

namespace binstarjs03.AerialOBJ.MvvmAppCore.Services.IOService;
public class RegionDiskLoader : IRegionDiskLoader
{
    private readonly GlobalState _globalState;
    private readonly IDispatcher _dispatcher;

    public RegionDiskLoader(GlobalState globalState, IDispatcher dispatcher)
    {
        _globalState = globalState;
        _dispatcher = dispatcher;
    }

    public bool TryGetRegion(PointZ<int> regionCoords, CancellationToken ct, [NotNullWhen(true)] out IRegion? region)
    {
        // Do not handle exception here, throw it and let caller decide what to do

        region = null;
        string? regionPath = getRegionPath(regionCoords);
        if (regionPath is null)
            return false;

        if (!File.Exists(regionPath))
            return false;

        FileStream fs = File.Open(regionPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

        // some region files have intentionally zero-length data
        // and make sure to check that before instantiating Region
        if (fs.Length == 0)
            return false;

        byte[] regionData = new byte[fs.Length];
        fs.Read(regionData, 0, regionData.Length);

        region = new Region(regionData, regionCoords);
        return true;

        string? getRegionPath(PointZ<int> regionCoords)
        {
            SavegameLoadInfo? loadInfo = _dispatcher.Invoke(() => _globalState.SavegameLoadInfo,
                                                            DispatcherPriority.Normal,
                                                            ct);
            if (loadInfo is null)
                return null;
            return Path.Combine(loadInfo.SavegameDirectoryPath, "region", $"r.{regionCoords.X}.{regionCoords.Z}.mca");
        }
    }
}
