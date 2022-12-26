using System;
using System.IO;

using binstarjs03.AerialOBJ.Core.MinecraftWorld;
using binstarjs03.AerialOBJ.Core.Primitives;
using binstarjs03.AerialOBJ.WpfAppNew2.Components;

namespace binstarjs03.AerialOBJ.WpfAppNew2.Services;
public class IOService : IIOService
{
    private readonly GlobalState _globalState;

    public IOService(GlobalState globalState)
    {
        _globalState = globalState;
    }

    public Region? ReadRegionFile(Point2Z<int> regionCoords, out Exception? e)
    {
        e = null;
        SavegameLoadInfo? loadInfo = _globalState.SavegameLoadInfo;
        if (loadInfo is null)
            return null;
        string regionFilePath = $"{loadInfo.SavegameDirectoryPath}/region/r.{regionCoords.X}.{regionCoords.Z}.mca";
        if (!File.Exists(regionFilePath))
            return null;
        try
        {
            return Region.Open(regionFilePath, regionCoords);
        }
        catch (Exception ex)
        {
            e = ex;
            return null;
        }
    }

    public bool WriteText(string path, string content, out Exception? e)
    {
        try
        {
            using StreamWriter writer = File.CreateText(path);
            writer.Write(content);
            e = null;
            return true;
        }
        catch (Exception ex)
        {
            e = ex;
            return false;
        }
    }
}
