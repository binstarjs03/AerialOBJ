using binstarjs03.AerialOBJ.Core.Primitives;
using binstarjs03.AerialOBJ.MvvmAppCore.Services.Input;

namespace binstarjs03.AerialOBJ.MvvmAppCore.ViewModels;
public static class ViewportInputHandlingConfiguration
{
    public static void ConfigureMouseHandler(ViewportViewModel viewmodel, IMouse mouse)
    {
        mouse.RegisterHandler(
            mouse => TranslateCameraWithMouse(viewmodel, mouse.MouseDelta),
            condition: mouse => mouse.IsMouseLeft && mouse.MouseDelta != PointY<int>.Zero,
            MouseWhen.MouseMove);

        mouse.RegisterHandler(
            mouse => ZoomWithMouse(viewmodel, mouse.ScrollDelta),
            condition: _ => true,
            MouseWhen.MouseWheel);
    }

    private static void TranslateCameraWithMouse(ViewportViewModel viewmodel, PointY<int> mouseDelta)
    {
        PointZ<float> cameraDelta = -(mouseDelta.ToFloat() / viewmodel.ZoomMultiplier);
        viewmodel.TranslateCamera(cameraDelta);
    }

    private static void ZoomWithMouse(ViewportViewModel viewmodel, int scrollDelta)
    {
        ZoomDirection direction = scrollDelta > 0 ? ZoomDirection.In : ZoomDirection.Out;
        viewmodel?.Zoom(direction);
    }
}
