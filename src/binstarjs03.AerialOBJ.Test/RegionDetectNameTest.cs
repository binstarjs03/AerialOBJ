using System;
using System.IO;

using binstarjs03.AerialOBJ.Core.CoordinateSystem;

namespace binstarjs03.AerialOBJ.Test;

[TestClass]
public class RegionDetectNameTest
{
    [TestMethod]
    public void DetectName()
    {
        string path = @"C:\Users\Bin\AppData\Roaming\.minecraft\saves\Terralith 2\region\r.1.3.mca";
        FileInfo fi = new(path);
        string[] split = fi.Name.Split('.');
        bool correctPrefix = split[0] == "r";
        bool correctFileType = split[3] == "mca";
        bool validCoordinate = int.TryParse(split[1], out _) && int.TryParse(split[2], out _);
        if (correctPrefix && correctFileType && validCoordinate)
        {
            int x = int.Parse(split[1]);
            int z = int.Parse(split[2]);
            Coords2 coords = new(x, z);
            Console.WriteLine(coords);
        }
        else
        {
            throw new Exception("Invalid File");
        }
    }
}
