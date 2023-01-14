using System.Threading;

using binstarjs03.AerialOBJ.Core.Primitives;
using binstarjs03.AerialOBJ.WpfApp.Components;

namespace binstarjs03.AerialOBJ.WpfApp.Factories;
public interface IRegionImageFactory
{
    IRegionImage Create(Point2Z<int> regionCoords, CancellationToken cancellationToken);
}
