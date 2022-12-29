using System.Threading;

using binstarjs03.AerialOBJ.Core.Primitives;
using binstarjs03.AerialOBJ.WpfAppNew2.Components;

namespace binstarjs03.AerialOBJ.WpfAppNew2.Factories;
public interface IMutableImageFactory
{
    IMutableImage Create(Size<int> size, CancellationToken cancellationToken);
}