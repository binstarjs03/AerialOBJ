using binstarjs03.AerialOBJ.Core.Primitives;

namespace binstarjs03.AerialOBJ.WpfApp.ViewModels;

// Abstraction for the input handler. we may abstract all viewmodels for consistency
public interface IViewportViewModel
{
    PointZ<float> CameraPos { get; }
    Size<int> ScreenSize { get; }
    PointZ<int> Selection1 { get; set; }
    PointZ<int> Selection2 { get; set; }

    // Actually, zoom and unit multipier ARE interchangeable.
    // The former is being used when manipulating zoom-related routine, while
    // the latter is being used when working with coordinate and space related routine
    float ZoomMultiplier { get; }
    float UnitMultiplier { get; }

    void TranslateCamera(PointZ<float> displacement);
    void Zoom(ZoomDirection direction);
    void MoveHeightLevel(HeightLevelDirection direction, int distance);
    void UpdateContextWorldInformation(PointZ<int> worldPos);
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