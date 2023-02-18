using System.Diagnostics.CodeAnalysis;
using System.Threading;

using binstarjs03.AerialOBJ.Core.MinecraftWorld;
using binstarjs03.AerialOBJ.Core.Primitives;

namespace binstarjs03.AerialOBJ.MVVM.Services.IOService;
public interface IRegionDiskLoader
{
    bool TryGetRegion(PointZ<int> regionCoords, CancellationToken ct, [NotNullWhen(true)] out IRegion? region);
}

