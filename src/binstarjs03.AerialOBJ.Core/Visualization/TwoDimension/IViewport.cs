using System;

using binstarjs03.AerialOBJ.Core.Primitives;

namespace binstarjs03.AerialOBJ.Core.Visualization.TwoDimension;
public interface IViewport
{
    Point2Z<float> CameraPos { get; set; }
    float ZoomLevel { get; set; }
    Size<int> ScreenSize { get; set; }

    void Update();
}
