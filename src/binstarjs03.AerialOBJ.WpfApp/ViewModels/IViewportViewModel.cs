using binstarjs03.AerialOBJ.Core.Primitives;

namespace binstarjs03.AerialOBJ.WpfApp.ViewModels;
public interface IViewportViewModel
{
    float ZoomMultiplier { get; }

    void TranslateCamera(PointZ<float> displacement);
    void Zoom(ZoomDirection direction);
    void MoveHeightLevel(HeightLevelDirection direction, int distance);
}

public enum ZoomDirection
{
    In,
    Out,
}

public enum HeightLevelDirection
{
    Up,
    Down,
}