using binstarjs03.AerialOBJ.Core.Primitives;

namespace binstarjs03.AerialOBJ.WpfApp.ExtensionMethods;
public static class Point2Extension
{
    public static PointY<float> Floor(this PointY<float> point)
    {
        return new PointY<float>(point.X.Floor(), point.Y.Floor());
    }
}
