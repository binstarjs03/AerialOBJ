using binstarjs03.AerialOBJ.Core.MinecraftWorld;
using binstarjs03.AerialOBJ.MVVM.Components;

namespace binstarjs03.AerialOBJ.MVVM.Models;
public class RegionDataImageModel
{
    public required IRegion Data { get; init; }
    public required IRegionImage Image { get; set; }
    public override string ToString()
    {
        return $"Region Data Image Model {Data.Coords}";
    }
}