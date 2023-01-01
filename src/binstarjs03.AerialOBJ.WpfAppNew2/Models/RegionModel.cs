using binstarjs03.AerialOBJ.Core.MinecraftWorldRefactor;
using binstarjs03.AerialOBJ.Core.Primitives;
using binstarjs03.AerialOBJ.WpfAppNew2.Components;

namespace binstarjs03.AerialOBJ.WpfAppNew2.Models;
public class RegionModel
{
    public required Region RegionData { get; init; }
    public required Point2Z<int> RegionCoords { get; init; }
    public Point2<float> WorldCoords => new(RegionCoords.X * Region.BlockCount, RegionCoords.Z * Region.BlockCount);
    public required IMutableImage RegionImage { get; init; }
}