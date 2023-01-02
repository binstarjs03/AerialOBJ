using System.Threading;

using binstarjs03.AerialOBJ.Core.MinecraftWorldRefactor;
using binstarjs03.AerialOBJ.Core.Primitives;
using binstarjs03.AerialOBJ.WpfApp.Models;

namespace binstarjs03.AerialOBJ.WpfApp.Factories;
public class RegionModelFactory
{
    private readonly IMutableImageFactory _mutableImageFactory;

    public RegionModelFactory(IMutableImageFactory mutableImageFactory)
    {
        _mutableImageFactory = mutableImageFactory;
    }

    public RegionModel Create(Point2Z<int> regionPosition, Region regionData, CancellationToken cancellationToken)
    {
        return new RegionModel()
        {
            RegionData = regionData,
            RegionCoords = regionPosition,
            RegionImage = _mutableImageFactory.Create(new Size<int>(Region.BlockCount, Region.BlockCount), cancellationToken),
        };
    }
}
