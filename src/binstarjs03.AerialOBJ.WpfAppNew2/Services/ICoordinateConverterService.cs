using binstarjs03.AerialOBJ.Core.Primitives;
using binstarjs03.AerialOBJ.WpfAppNew2.Components;

namespace binstarjs03.AerialOBJ.WpfAppNew2.Services;
public interface ICoordinateConverterService
{
    Point2<float> ConvertWorldToScreen(Point2<float> worldCoord, ViewportArg viewportArg);
}