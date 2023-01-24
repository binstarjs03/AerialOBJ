using System.Threading;
using System.Threading.Tasks;

using binstarjs03.AerialOBJ.Core.Primitives;
using binstarjs03.AerialOBJ.WpfApp.Components;
using binstarjs03.AerialOBJ.WpfApp.Services.Dispatcher;

namespace binstarjs03.AerialOBJ.WpfApp.Factories;
public class RegionImageFactory : IRegionImageFactory
{
    private readonly IDispatcher _dispatcher;

    public RegionImageFactory(IDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    public IRegionImage Create(PointZ<int> regionCoords, CancellationToken cancellationToken)
    {
        if (_dispatcher.CheckAccess())
            return new RegionImage(regionCoords);
        RegionImage? result = _dispatcher.Invoke(() => new RegionImage(regionCoords), DispatcherPriority.Background, cancellationToken);
        if (result is null)
            throw new TaskCanceledException();
        return result;
    }
}
