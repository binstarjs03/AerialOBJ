using binstarjs03.AerialOBJ.Core.Primitives;
using binstarjs03.AerialOBJ.WpfAppNew2.Components;
using binstarjs03.AerialOBJ.WpfAppNew2.ExtensionMethods;

namespace binstarjs03.AerialOBJ.WpfAppNew2.Services;
public class CoordinateConverterService : ICoordinateConverterService
{
    public Point2<float> ConvertWorldToScreen(Point2<float> worldCoord, ViewportArg viewportArg)
    {
        Point2<float> unitOffsetScaled = (Point2<float>)(-(viewportArg.CameraPos * viewportArg.ZoomLevel));
        Point2<float> screenCenter = new (viewportArg.ScreenSize.Width / 2,
                                          viewportArg.ScreenSize.Height / 2);

        Point2<float> scaledPoint = worldCoord * viewportArg.ZoomLevel;
        Point2<float> screenPoint = (unitOffsetScaled + screenCenter + scaledPoint).Floor();

        return screenPoint;
    }
}
