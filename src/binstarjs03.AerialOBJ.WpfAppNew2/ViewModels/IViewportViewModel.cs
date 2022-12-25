using binstarjs03.AerialOBJ.Core.Primitives;

namespace binstarjs03.AerialOBJ.WpfAppNew2.ViewModels;
public interface IViewportViewModel
{
    Point2Z<float> CameraPos { get; set; }
    Size<int> ScreenSize { get; set; }
    float UnitMultiplier { get; }
    int ZoomLevel { get; }
}
