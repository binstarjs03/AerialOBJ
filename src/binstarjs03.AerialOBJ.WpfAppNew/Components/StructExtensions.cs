using System.Windows;

using binstarjs03.AerialOBJ.Core;

namespace binstarjs03.AerialOBJ.WpfAppNew.Components;

public static class StructExtensions
{
    public static Point GetFloor(this Point point)
    {
        return new Point(MathUtils.Floor(point.X), MathUtils.Floor(point.Y));
    }

    public static Size GetFloor(this Size size)
    {
        return new Size(MathUtils.Floor(size.Width), MathUtils.Floor(size.Height));
    }

    public static Vector GetFloor(this Vector vector)
    {
        return new Vector(MathUtils.Floor(vector.X), MathUtils.Floor(vector.Y));
    }

    public static Point Add(this Point left, Point right)
    {
        return new Point(left.X + right.X, left.Y + right.Y);
    }
}
