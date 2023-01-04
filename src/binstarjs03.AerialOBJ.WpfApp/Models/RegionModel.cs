using binstarjs03.AerialOBJ.Core.MinecraftWorldRefactor;
using binstarjs03.AerialOBJ.Core.Primitives;
using binstarjs03.AerialOBJ.WpfApp.Components;

namespace binstarjs03.AerialOBJ.WpfApp.Models;
public class RegionModel
{
    public required Region RegionData { get; init; }
    public Point2<float> WorldCoords => new(RegionData.Coords.X * Region.BlockCount, RegionData.Coords.Z * Region.BlockCount);
    public required IMutableImage RegionImage { get; init; }
}