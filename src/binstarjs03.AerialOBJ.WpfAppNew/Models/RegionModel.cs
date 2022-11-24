using System.Collections.Generic;

using binstarjs03.AerialOBJ.Core.MinecraftWorld;
using binstarjs03.AerialOBJ.Core.Primitives;
using binstarjs03.AerialOBJ.WpfAppNew.Components;
using binstarjs03.AerialOBJ.WpfAppNew.Components.Interfaces;

namespace binstarjs03.AerialOBJ.WpfAppNew.Models;

public class RegionModel
{
    public Region Region { get; }
    public HashSet<Point2Z<int>> GeneratedChunks { get; }
    public Point2Z<int> RegionCoords { get; }
    public RegionImage RegionImage { get; }

    public RegionModel(Region region)
    {
        Region = region;
        GeneratedChunks = region.GetGeneratedChunksAsCoordsRelSet();
        RegionCoords = region.RegionCoords;
        RegionImage = new RegionImage();
    }
}
