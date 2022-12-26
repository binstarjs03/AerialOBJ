using binstarjs03.AerialOBJ.Core.MinecraftWorld;
using binstarjs03.AerialOBJ.Core.Primitives;
using binstarjs03.AerialOBJ.WpfAppNew2.Models;

namespace binstarjs03.AerialOBJ.WpfAppNew2.Factories;
public class RegionImageModelFactory
{
    private readonly IMutableImageFactory _mutableImageFactory;

    public RegionImageModelFactory(IMutableImageFactory mutableImageFactory)
    {
        _mutableImageFactory = mutableImageFactory;
    }

    public RegionImageModel Create(Point2Z<int> regionPosition)
    {
        return new RegionImageModel()
        {
            RegionPosition = regionPosition,
            Image = _mutableImageFactory.Create(new Size<int>(Region.BlockCount, Region.BlockCount)),
        };
    }
}
