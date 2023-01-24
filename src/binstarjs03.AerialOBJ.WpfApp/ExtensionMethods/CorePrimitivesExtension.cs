using System.Windows;

using binstarjs03.AerialOBJ.Core;
using binstarjs03.AerialOBJ.Core.Primitives;
using binstarjs03.AerialOBJ.WpfApp;

namespace binstarjs03.AerialOBJ.WpfApp;
public static class CorePrimitivesExtension
{
    public static PointY<float> Floor(this PointY<float> point)
    {
        return new PointY<float>(point.X.Floor(), point.Y.Floor());
    }

    public static PointY<int> Floor(this Point point)
    {
        return new PointY<int>(point.X.Floor(), point.Y.Floor());
    }

    public static PointZ<int> Floor(this PointZ<float> point)
    {
        return new PointZ<int>(point.X.Floor(), point.Z.Floor());
    }

    public static PointY<float> ToFloat(this PointY<int> point)
    {
        return new PointY<float>(point.X, point.Y);
    }

    public static Size<float> ToFloat(this Size<int> size)
    {
        return new Size<float>(size.Width, size.Height);
    }
}
