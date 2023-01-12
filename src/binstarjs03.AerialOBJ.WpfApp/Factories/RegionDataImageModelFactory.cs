using System.Threading;

using binstarjs03.AerialOBJ.Core.MinecraftWorld;
using binstarjs03.AerialOBJ.WpfApp.Models;

namespace binstarjs03.AerialOBJ.WpfApp.Factories;
public class RegionDataImageModelFactory
{
    private readonly IRegionImageFactory _regionImageFactory;

    public RegionDataImageModelFactory(IRegionImageFactory regionImageFactory)
    {
        _regionImageFactory = regionImageFactory;
    }

    public RegionDataImageModel Create(IRegion regionData, CancellationToken cancellationToken)
    {
        return new RegionDataImageModel()
        {
            Data = regionData,
            Image = _regionImageFactory.Create(regionData.Coords, cancellationToken),
        };
    }
}
