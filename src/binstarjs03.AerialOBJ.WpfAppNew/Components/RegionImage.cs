using System.Windows.Controls;

using binstarjs03.AerialOBJ.Core.Primitives;
using binstarjs03.AerialOBJ.WpfAppNew.Components.Interfaces;

namespace binstarjs03.AerialOBJ.WpfAppNew.Components;

public class RegionImage : Image, IRegionImage
{
    public void SetPixel(Point2<int> pixel, IColor color)
    {
        throw new System.NotImplementedException();
    }
}
