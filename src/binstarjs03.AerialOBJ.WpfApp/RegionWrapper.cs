using binstarjs03.AerialOBJ.Core.CoordinateSystem;
using binstarjs03.AerialOBJ.Core.WorldRegion;
using binstarjs03.AerialOBJ.WpfApp.Services;

namespace binstarjs03.AerialOBJ.WpfApp;

public class RegionWrapper
{
    private readonly Coords2 _regionCoordsAbs;
    private Region? _region;

    public Region? Region => _region;

    public RegionWrapper(Coords2 regionCoordsAbs)
    {
        _regionCoordsAbs = regionCoordsAbs;
    }

    public void Allocate()
    {
        OnAllocate();
    }

    public void Deallocate()
    {
        if (_region is null)
            return;
        _region.Dispose();
        _region = null;
    }

    public void OnAllocate()
    {
        _region = IOService.LoadRegion(_regionCoordsAbs);
    }
}
