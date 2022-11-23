using System.Collections.Generic;

using binstarjs03.AerialOBJ.Core.MinecraftWorld;
using binstarjs03.AerialOBJ.Core.Primitives;

namespace binstarjs03.AerialOBJ.Core.Visualization.TwoDimension;

public class RegionModel<TRegionImage> where TRegionImage : class, IRegionImage, new()
{
    public Region Region { get; }
    public HashSet<Point2Z<int>> GeneratedChunks { get; }
    public Point2Z<int> RegionCoords { get; }
    public TRegionImage RegionImage { get; }

    public RegionModel(Region region)
    {
        Region = region;
        GeneratedChunks = region.GetGeneratedChunksAsCoordsRelSet();
        RegionCoords = region.RegionCoords;
        RegionImage = new TRegionImage();
    }
}
