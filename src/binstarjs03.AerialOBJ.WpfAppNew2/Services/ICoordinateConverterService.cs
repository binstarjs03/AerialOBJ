using binstarjs03.AerialOBJ.Core.Primitives;

namespace binstarjs03.AerialOBJ.WpfAppNew2.Services;
public interface ICoordinateConverterService
{
    Point2<float> ConvertWorldToScreen(Point2Z<float> worldPos, Point2Z<float> cameraPos, float unitMultiplier, Size<int> screenSize);
    float ConvertWorldToScreen(float worldPos, float cameraPos, float unitMultiplier, int screenSize);
}