using System.Threading;
using System.Threading.Tasks;

using binstarjs03.AerialOBJ.Core.Pooling;
using binstarjs03.AerialOBJ.Core.Primitives;
using binstarjs03.AerialOBJ.WpfApp.Components;
using binstarjs03.AerialOBJ.WpfApp.Factories;

namespace binstarjs03.AerialOBJ.WpfApp.Services;

public class RegionImagePooler : IRegionImagePooler
{
    private readonly ObjectPool<IRegionImage> _pool = new();
    private readonly IRegionImageFactory _regionImageFactory;

    public RegionImagePooler(IRegionImageFactory regionImageFactory, CancellationToken ct)
    {
        _regionImageFactory = regionImageFactory;

        // prepare 30 premade region images
        for (int i = 0; i < 30; i++)
        {
            IRegionImage regionImage = _regionImageFactory.Create(Point2Z<int>.Zero, ct);
            _pool.Return(regionImage);
        }
    }

    public IRegionImage Rent(Point2Z<int> regionCoords, CancellationToken ct)
    {
        lock (_pool)
            if (_pool.Rent(out IRegionImage? result))
            {
                result.RegionCoords = regionCoords;
                return result;
            }
        return _regionImageFactory.Create(regionCoords, ct);
    }

    public void Return(IRegionImage image)
    {
        Task.Run(() =>
        {
            CleanImage(image);
            lock (_pool)
                _pool.Return(image);
        });
    }

    private static void CleanImage(IRegionImage image)
    {
        for (int x = 0; x < image.Size.Width; x++)
            for (int y = 0; y < image.Size.Height; y++)
            {
                image[x, y] = NamedColors.Transparent;
            }
        image.Redraw();
    }
}