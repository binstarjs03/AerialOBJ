using binstarjs03.AerialOBJ.Core.Primitives;

namespace binstarjs03.AerialOBJ.WpfAppNew2.Components;
public readonly struct ViewportArg
{
    public required Point2Z<float> CameraPos { get; init; }
    public required Size<int> ScreenSize { get; init; }
    public required float ZoomLevel { get; init; }
}
