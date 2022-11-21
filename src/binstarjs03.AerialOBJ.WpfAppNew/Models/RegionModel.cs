using System.Collections.Generic;

using binstarjs03.AerialOBJ.Core.CoordinateSystem;
using binstarjs03.AerialOBJ.Core.MinecraftWorld;

namespace binstarjs03.AerialOBJ.WpfAppNew.Models;
public class RegionModel
{
    public Region Region { get; }
    public HashSet<Coords2> GeneratedChunks { get; }
    public RegionModel(Region region)
    {
        Region = region;
        GeneratedChunks = region.GetGeneratedChunksAsCoordsRelSet();
    }
}
