using binstarjs03.AerialOBJ.Core.Primitives;

namespace binstarjs03.AerialOBJ.WpfApp.Components;
public interface IRegionImage: IMutableImage
{
    public Point2<float> ImagePosition { get; }
}
