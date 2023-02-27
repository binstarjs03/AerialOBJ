using System.Threading;
using System.Windows.Threading;

using binstarjs03.AerialOBJ.Core.MinecraftWorld;
using binstarjs03.AerialOBJ.MvvmAppCore.Factories;
using binstarjs03.AerialOBJ.MvvmAppCore.Models;
using binstarjs03.AerialOBJ.WpfApp.Components;

namespace binstarjs03.AerialOBJ.WpfApp.Factories;

public class RegionDataImageModelFactory : IRegionDataImageModelFactory
{
    public RegionDataImageModel Create(IRegion regionData, CancellationToken cancellationToken)
    {
        RegionImage image = App.Current.Dispatcher.Invoke(() => new RegionImage(regionData.Coords), DispatcherPriority.Background, cancellationToken);

        return new RegionDataImageModel
        {
            Data = regionData,
            Image = image
        };
    }
}
