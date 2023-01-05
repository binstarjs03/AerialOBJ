using System.Threading;

using binstarjs03.AerialOBJ.Core.MinecraftWorldRefactor;
using binstarjs03.AerialOBJ.Core.Primitives;
using binstarjs03.AerialOBJ.WpfApp.Models;

namespace binstarjs03.AerialOBJ.WpfApp.Factories;
public class RegionModelFactory
{
    private readonly IRegionImageFactory _regionImageFactory;

    public RegionModelFactory(IRegionImageFactory regionImageFactory)
    {
        _regionImageFactory = regionImageFactory;
    }

    public RegionModel Create(Region regionData, CancellationToken cancellationToken)
    {
        return new RegionModel()
        {
            Data = regionData,
            Image = _regionImageFactory.Create(regionData.Coords, cancellationToken),
        };
    }
}
