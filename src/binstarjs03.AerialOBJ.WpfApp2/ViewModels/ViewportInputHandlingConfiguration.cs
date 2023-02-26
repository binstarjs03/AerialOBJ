using System.Windows.Input;

using binstarjs03.AerialOBJ.Core.Primitives;
using binstarjs03.AerialOBJ.MvvmAppCore.Services.Input;
using binstarjs03.AerialOBJ.MvvmAppCore.ViewModels;

namespace binstarjs03.AerialOBJ.WpfApp.ViewModels;
public static class ViewportInputHandlingConfiguration
{
    public static void ConfigureMouseHandler(ViewportViewModel viewmodel, IMouse mouse)
    {
        // move camera
        mouse.RegisterHandler(
            mouse => TranslateCameraWithMouse(viewmodel, mouse.MouseDelta),
            condition: mouse => mouse.IsMouseLeft && mouse.MouseDelta != PointY<int>.Zero,
            MouseWhen.MouseMove);

        // zoom in/out
        mouse.RegisterHandler(
            mouse => ZoomWithMouse(viewmodel, mouse.ScrollDelta),
            condition: _ => true,
            MouseWhen.MouseWheel);

        // context world
        mouse.RegisterHandler(
            mouse => UpdateContextWorld(viewmodel, mouse),
            condition: _ => true,
            MouseWhen.MouseMove);
    }

    public static void ConfigureKeyboardHandler(ViewportViewModel viewmodel, IKeyboard keyboard)
    {
        // directional key, translate camera
        keyboard.RegisterKeyDownHandler(Key.Left, () => TranslateCameraWithKey(viewmodel, PointZ<float>.Left));
        keyboard.RegisterKeyDownHandler(Key.Up, () => TranslateCameraWithKey(viewmodel, PointZ<float>.Front));
        keyboard.RegisterKeyDownHandler(Key.Right, () => TranslateCameraWithKey(viewmodel, PointZ<float>.Right));
        keyboard.RegisterKeyDownHandler(Key.Down, () => TranslateCameraWithKey(viewmodel, PointZ<float>.Back));

        // zoom key
        keyboard.RegisterKeyDownHandler(Key.OemPlus, () => viewmodel.Zoom(ZoomDirection.In));
        keyboard.RegisterKeyDownHandler(Key.OemMinus, () => viewmodel.Zoom(ZoomDirection.Out));

        // height level key
        keyboard.RegisterKeyDownHandler(Key.OemPeriod, () => viewmodel.MoveHeightLevel(HeightLevelDirection.Up, 1));
        keyboard.RegisterKeyDownHandler(Key.OemComma, () => viewmodel.MoveHeightLevel(HeightLevelDirection.Down, 1));
    }

    private static void TranslateCameraWithMouse(ViewportViewModel viewmodel, PointY<int> mouseDelta)
    {
        PointZ<float> cameraDelta = -(mouseDelta.ToFloat() / viewmodel.ZoomMultiplier);
        viewmodel.TranslateCamera(cameraDelta);
    }

    private static void TranslateCameraWithKey(ViewportViewModel viewModel, PointZ<float> direction)
    {
        viewModel.TranslateCamera(direction * 50 / viewModel.ZoomMultiplier);
    }

    private static void ZoomWithMouse(ViewportViewModel viewmodel, int scrollDelta)
    {
        ZoomDirection direction = scrollDelta > 0 ? ZoomDirection.In : ZoomDirection.Out;
        viewmodel?.Zoom(direction);
    }

    private static void UpdateContextWorld(ViewportViewModel viewmodel, IMouse mouse)
    {
        PointZ<int> worldPos = ConvertMousePosToWorldPos(viewmodel, mouse);
        viewmodel.UpdateContextWorldInformation(worldPos);
    }

    private static PointZ<int> ConvertMousePosToWorldPos(ViewportViewModel viewmodel, IMouse mouse)
    {
        return PointSpaceConversion.ConvertScreenPosToWorldPos(
            mouse.MousePos.ToFloat(),
            viewmodel.CameraPos,
            viewmodel.ZoomMultiplier,
            viewmodel.ScreenSize.ToFloat()).Floor();
    }
}
