using binstarjs03.AerialOBJ.Core.Primitives;
using binstarjs03.AerialOBJ.WpfAppNew2.ExtensionMethods;
using binstarjs03.AerialOBJ.WpfAppNew2.ViewModels;

namespace binstarjs03.AerialOBJ.WpfAppNew2.Services;
public class CoordinateConverterService : ICoordinateConverterService
{
    public Point2<float> ConvertWorldToScreen(Point2<float> worldCoord, IViewportViewModel viewport)
    {
        Point2<float> unitOffsetScaled = (Point2<float>)(-(viewport.CameraPos * viewport.ZoomLevel));
        Point2<float> screenCenter = new (viewport.ScreenSize.Width / 2,
                                          viewport.ScreenSize.Height / 2);

        Point2<float> scaledPoint = worldCoord * viewport.ZoomLevel;
        Point2<float> screenPoint = (unitOffsetScaled + screenCenter + scaledPoint).Floor();

        return screenPoint;
    }

    public float ConvertWorldToScreen(float worldPos, float cameraPos, float zoomLevel, int screenSize)
    {
        float unitOffsetScaled = -(cameraPos * zoomLevel);
        float screenCenter = screenSize / 2f;
        float scaledPos = worldPos * zoomLevel;
        float screenPos = (unitOffsetScaled + screenCenter + scaledPos).Floor();
        return screenPos;
    }
}
