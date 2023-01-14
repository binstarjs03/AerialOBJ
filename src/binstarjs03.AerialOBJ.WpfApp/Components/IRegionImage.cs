using binstarjs03.AerialOBJ.Core.Primitives;

namespace binstarjs03.AerialOBJ.WpfApp.Components;
public interface IRegionImage : IMutableImage
{
    Point2Z<int> RegionCoords { get; set; }
    Point2<float> ImagePosition { get; }
}
