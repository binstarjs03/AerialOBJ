using binstarjs03.AerialOBJ.Core.MinecraftWorld;
using binstarjs03.AerialOBJ.Core.Primitives;

namespace binstarjs03.AerialOBJ.WpfApp.Components;
public class RegionImage : MutableImage, IRegionImage
{
    private Point2Z<int> _regionCoords;

    public RegionImage(Point2Z<int> regionCoords) : base(new Size<int>(Region.BlockCount, Region.BlockCount))
    {
        RegionCoords = regionCoords;
    }

    public Point2Z<int> RegionCoords
    {
        get => _regionCoords;
        set
        {
            _regionCoords = value;
            ImagePosition = new Point2<float>(_regionCoords.X * Region.BlockCount, _regionCoords.Z * Region.BlockCount);
        }
    }
    public Point2<float> ImagePosition { get; private set; }
}
