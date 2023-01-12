using System.Threading;

using binstarjs03.AerialOBJ.Core.Primitives;
using binstarjs03.AerialOBJ.WpfApp.Components;

namespace binstarjs03.AerialOBJ.WpfApp.Services;
public interface IRegionImagePooler
{
    IRegionImage Rent(Point2Z<int> regionCoords, CancellationToken ct);
    void Return(IRegionImage image);
}
