using System.Threading;

using binstarjs03.AerialOBJ.Core.MinecraftWorld;
using binstarjs03.AerialOBJ.MvvmAppCore.Models;

namespace binstarjs03.AerialOBJ.MvvmAppCore.Factories;

public interface IRegionDataImageModelFactory
{
    RegionDataImageModel Create(IRegion regionData, CancellationToken cancellationToken);
}