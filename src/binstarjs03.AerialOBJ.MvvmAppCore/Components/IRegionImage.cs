using binstarjs03.AerialOBJ.Core.Primitives;

namespace binstarjs03.AerialOBJ.MvvmAppCore.Components;

// extend IRegionImage, it has additional information to position
// image of region correctly into canvas
public interface IRegionImage : IMutableImage
{
    PointZ<int> RegionCoords { get; set; }
    PointY<float> ImagePosition { get; }
}
