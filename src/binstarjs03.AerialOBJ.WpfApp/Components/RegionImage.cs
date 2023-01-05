using binstarjs03.AerialOBJ.Core.MinecraftWorldRefactor;
using binstarjs03.AerialOBJ.Core.Primitives;

namespace binstarjs03.AerialOBJ.WpfApp.Components;
public class RegionImage : MutableImage, IRegionImage
{
    public RegionImage(Point2Z<int> regionCoords) : base(new Size<int>(Region.BlockCount, Region.BlockCount))
    {
        ImagePosition = new Point2<float>(regionCoords.X * Region.BlockCount, regionCoords.Z * Region.BlockCount);
    }

    public Point2<float> ImagePosition { get; }
}
