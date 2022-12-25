using binstarjs03.AerialOBJ.Core.Primitives;
using binstarjs03.AerialOBJ.WpfAppNew2.ViewModels;

namespace binstarjs03.AerialOBJ.WpfAppNew2.Services;
public interface ICoordinateConverterService
{
    Point2<float> ConvertWorldToScreen(Point2Z<float> worldPos, Point2Z<float> cameraPos, float zoomLevel, Size<int> screenSize);
    float ConvertWorldToScreen(float worldPos, float cameraPos, float zoomLevel, int screenSize);
}