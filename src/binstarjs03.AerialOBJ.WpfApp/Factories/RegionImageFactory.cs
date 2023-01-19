using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

using binstarjs03.AerialOBJ.Core.Primitives;
using binstarjs03.AerialOBJ.WpfApp.Components;

namespace binstarjs03.AerialOBJ.WpfApp.Factories;
public class RegionImageFactory : IRegionImageFactory
{
    public IRegionImage Create(PointZ<int> regionCoords, CancellationToken cancellationToken)
    {
        if (App.Current.CheckAccess())
            return new RegionImage(regionCoords);
        RegionImage? result = App.Current.Dispatcher.Invoke(() => new RegionImage(regionCoords), DispatcherPriority.Background, cancellationToken);
        if (result is null)
            throw new TaskCanceledException();
        return result;
    }
}
