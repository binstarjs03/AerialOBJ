using binstarjs03.AerialOBJ.Core.MinecraftWorld;
using binstarjs03.AerialOBJ.WpfApp.Components;

namespace binstarjs03.AerialOBJ.WpfApp.Models;
public class RegionModel
{
    public required Region Data { get; init; }
    public required IRegionImage Image { get; init; }
}