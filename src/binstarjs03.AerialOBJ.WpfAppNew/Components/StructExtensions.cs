using System.Windows;

using binstarjs03.AerialOBJ.Core;
using binstarjs03.AerialOBJ.Core.Primitives;

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

    public static Size<int> ToCoreSize(this Size wpfSize)
    {
        return new Size<int>(MathUtils.Floor(wpfSize.Width), 
                             MathUtils.Floor(wpfSize.Height));
    }

    public static Vector2<float> ToCoreVector(this Vector wpfVector)
    {
        return new Vector2<float>((float)wpfVector.X, (float)wpfVector.Y);
    }

    public static Vector2Z<float> ToCoreVectorZ(this Vector wpfVector)
    {
        return new Vector2Z<float>((float)wpfVector.X, (float)wpfVector.Y);
    }
}
