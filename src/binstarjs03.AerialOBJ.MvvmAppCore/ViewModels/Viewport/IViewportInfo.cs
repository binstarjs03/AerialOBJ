using System;

using binstarjs03.AerialOBJ.Core.Primitives;

namespace binstarjs03.AerialOBJ.MvvmAppCore.ViewModels.Viewport;
public interface IViewportInfo
{
    Size<int> ScreenSize { get; set; }
    PointZ<float> CameraPos { get; set; }
    float ZoomMultiplier { get; set; }
    int HeightLevel { get; }

    event Action<Size<int>> ScreenSizeChanged;
    event Action<PointZ<float>> CameraPosChanged;
    event Action<float> ZoomMultiplierChanged;
    event Action<int> HeightLevelChanged;
}
