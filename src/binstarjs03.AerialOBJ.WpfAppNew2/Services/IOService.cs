using System;
using System.IO;

using binstarjs03.AerialOBJ.Core.MinecraftWorldRefactor;
using binstarjs03.AerialOBJ.Core.Primitives;
using binstarjs03.AerialOBJ.WpfAppNew2.Components;

namespace binstarjs03.AerialOBJ.WpfAppNew2.Services;
public class IOService : IIOService
{
    // Passing SavegameLoadInfo manually makes it thread-safe
    // since the only way to obtain it safely is through the UI Thread
    public Region? ReadRegionFile(Point2Z<int> regionCoords, SavegameLoadInfo loadInfo, out Exception? e)
    {
        e = null;
        string regionFilePath = $"""{loadInfo.SavegameDirectoryPath}\region\r.{regionCoords.X}.{regionCoords.Z}.mca""";
        if (!File.Exists(regionFilePath))
            return null;
        try
        {
            return new Region(regionFilePath, regionCoords);
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
