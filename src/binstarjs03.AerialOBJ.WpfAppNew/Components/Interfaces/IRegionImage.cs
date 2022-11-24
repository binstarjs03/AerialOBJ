using binstarjs03.AerialOBJ.Core.Primitives;

namespace binstarjs03.AerialOBJ.WpfAppNew.Components.Interfaces;

public interface IRegionImage
{
    void SetPixel(Point2<int> pixel, IColor color);
}
