using System;

using binstarjs03.AerialOBJ.WpfAppNew2.Components;

namespace binstarjs03.AerialOBJ.WpfAppNew2.Services;
public interface IChunkRegionManagerService<T> where T : class, IMutableImage
{
    event Action<T> RegionImageAdded;
    event Action<T> RegionImageRemoved;
    void AddRegionImage();
    void RemoveRegionImage();
}
