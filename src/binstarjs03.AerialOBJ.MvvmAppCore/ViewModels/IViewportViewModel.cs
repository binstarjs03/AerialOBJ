using System;

using binstarjs03.AerialOBJ.Core.Primitives;

namespace binstarjs03.AerialOBJ.MvvmAppCore.ViewModels;
public interface IViewportViewModel
{
    PointZ<float> CameraPos { get; set; }
    int HeightLevel { get; set; }

    event Action CameraPosChanged;
    event Action HeightLevelChanged;
}
