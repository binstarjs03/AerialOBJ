using System.Threading;

using binstarjs03.AerialOBJ.Core.Primitives;
using binstarjs03.AerialOBJ.WpfApp.Components;

namespace binstarjs03.AerialOBJ.WpfApp.Factories;
public interface IMutableImageFactory
{
    IMutableImage Create(Size<int> size, CancellationToken cancellationToken);
}