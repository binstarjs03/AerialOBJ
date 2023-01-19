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

    public bool TryGetRegion(PointZ<int> regionCoords, CancellationToken ct, [NotNullWhen(true)] out IRegion? region)
    {
        // Do not handle exception here, throw it and let caller decide what to do

        region = null;
        string? regionPath = getRegionPath(regionCoords);
        if (regionPath is null)
            return false;

        // exception is expensive, we don't want to throw because we are
        // frequently accessing files that may exist or not, and we can
        // check if file exist beforehand
        if (!File.Exists(regionPath))
            return false;

        // Previously we were using File.ReadAllBytes but that API can't read file 
        // if other processes opening that file too, so we refactored it to File.Open
        // with sharing access set to Read and Write so we can access the file,
        // even though we are playing the savegame in Minecraft
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
            SavegameLoadInfo? loadInfo = _dispatcher.Invoke(() => _globalState.SavegameLoadInfo, DispatcherPriority.Normal, ct);
            if (loadInfo is null)
                return null;
            return Path.Combine(loadInfo.SavegameDirectoryPath, "region", $"r.{regionCoords.X}.{regionCoords.Z}.mca");
        }
    }
}
