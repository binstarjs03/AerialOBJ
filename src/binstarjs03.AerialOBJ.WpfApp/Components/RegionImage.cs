using binstarjs03.AerialOBJ.Core.MinecraftWorld;
using binstarjs03.AerialOBJ.Core.Primitives;
using binstarjs03.AerialOBJ.MvvmAppCore.Components;

namespace binstarjs03.AerialOBJ.WpfApp.Components;
public class RegionImage : MutableImage, IRegionImage
{
    private PointZ<int> _regionCoords;

    public RegionImage(PointZ<int> regionCoords) : base(new Size<int>(IRegion.BlockCount, IRegion.BlockCount))
    {
        RegionCoords = regionCoords;
    }

    public PointZ<int> RegionCoords
    {
        get => _regionCoords;
        set
        {
            _regionCoords = value;
            ImagePosition = new PointY<float>(_regionCoords.X * IRegion.BlockCount, _regionCoords.Z * IRegion.BlockCount);
        }
    }
    public PointY<float> ImagePosition { get; private set; }
}
