using System.Windows;

using binstarjs03.AerialOBJ.Core;
using binstarjs03.AerialOBJ.Core.Primitives;
using binstarjs03.AerialOBJ.WpfApp;

namespace binstarjs03.AerialOBJ.WpfApp;
public static class CorePrimitivesExtension
{
    public static PointY<int> Floor(this Point point)
    {
        return new PointY<int>(point.X.Floor(), point.Y.Floor());
    }
}
