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
}
