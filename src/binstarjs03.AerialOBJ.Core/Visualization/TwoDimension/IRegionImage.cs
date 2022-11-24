using binstarjs03.AerialOBJ.Core.Primitives;

namespace binstarjs03.AerialOBJ.Core.Visualization.TwoDimension;

public interface IRegionImage
{
    void SetPixel(Point2<int> pixel, IColor color);
}
