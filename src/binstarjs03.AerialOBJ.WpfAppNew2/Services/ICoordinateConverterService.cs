using binstarjs03.AerialOBJ.Core.Primitives;
using binstarjs03.AerialOBJ.WpfAppNew2.ViewModels;

namespace binstarjs03.AerialOBJ.WpfAppNew2.Services;
public interface ICoordinateConverterService
{
    Point2<float> ConvertWorldToScreen(Point2<float> worldCoord, IViewportViewModel viewport);
    float ConvertWorldToScreen(float worldPos, float cameraPos, float zoomLevel, int screenSize);
}