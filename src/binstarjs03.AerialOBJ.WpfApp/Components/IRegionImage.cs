using binstarjs03.AerialOBJ.Core.Primitives;

namespace binstarjs03.AerialOBJ.WpfApp.Components;
public interface IRegionImage : IMutableImage
{
    PointZ<int> RegionCoords { get; set; }
    PointY<float> ImagePosition { get; }
}
