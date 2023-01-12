using System.Diagnostics.CodeAnalysis;
using System.Threading;

using binstarjs03.AerialOBJ.Core.MinecraftWorld;
using binstarjs03.AerialOBJ.Core.Primitives;

namespace binstarjs03.AerialOBJ.WpfApp.Services.ChunkRegionProvider;
public interface IRegionDiskLoader
{
    bool TryGetRegion(Point2Z<int> regionCoords, CancellationToken ct, [NotNullWhen(true)] out IRegion? region);
}

