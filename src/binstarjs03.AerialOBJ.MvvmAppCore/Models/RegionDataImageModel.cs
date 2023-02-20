using binstarjs03.AerialOBJ.Core.MinecraftWorld;
using binstarjs03.AerialOBJ.MvvmAppCore.Components;

namespace binstarjs03.AerialOBJ.MvvmAppCore.Models;
public class RegionDataImageModel
{
    public required IRegion Data { get; init; }
    public required IRegionImage Image { get; set; }
    public override string ToString()
    {
        return $"Region Data Image Model {Data.Coords}";
    }
}