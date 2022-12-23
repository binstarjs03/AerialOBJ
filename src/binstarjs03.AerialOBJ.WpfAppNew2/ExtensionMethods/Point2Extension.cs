using binstarjs03.AerialOBJ.Core.Primitives;

namespace binstarjs03.AerialOBJ.WpfAppNew2.ExtensionMethods;
public static class Point2Extension
{
    public static Point2<float> Floor(this Point2<float> point)
    {
        return new Point2<float>(point.X.Floor(), point.Y.Floor());
    }
}
