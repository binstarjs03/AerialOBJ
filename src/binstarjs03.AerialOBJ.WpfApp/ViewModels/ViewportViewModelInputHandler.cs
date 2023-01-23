using System.Windows.Input;

using binstarjs03.AerialOBJ.Core;
using binstarjs03.AerialOBJ.Core.Primitives;
using binstarjs03.AerialOBJ.WpfApp.ExtensionMethods;
using binstarjs03.AerialOBJ.WpfApp.Services.Input;

namespace binstarjs03.AerialOBJ.WpfApp.ViewModels;
public class ViewportViewModelInputHandler
{
    public ViewportViewModelInputHandler(IKeyHandler keyHandler, IMouseHandler mouseHandler)
    {
        KeyHandler = keyHandler;
        MouseHandler = mouseHandler;

        ConfigureKeyHandler();
        ConfigureMouseHandler();
    }

    public IViewportViewModel? Viewport { get; set; }
    public IKeyHandler KeyHandler { get; }
    public IMouseHandler MouseHandler { get; }

    private void ConfigureKeyHandler()
    {
        // directional key
        KeyHandler.RegisterKeyDownHandler(Key.Left, () => TranslateCameraWithKey(PointZ<float>.Left));
        KeyHandler.RegisterKeyDownHandler(Key.Up, () => TranslateCameraWithKey(PointZ<float>.Front));
        KeyHandler.RegisterKeyDownHandler(Key.Right, () => TranslateCameraWithKey(PointZ<float>.Right));
        KeyHandler.RegisterKeyDownHandler(Key.Down, () => TranslateCameraWithKey(PointZ<float>.Back));

        // zoom key
        KeyHandler.RegisterKeyDownHandler(Key.OemPlus, () => Viewport?.Zoom(ZoomDirection.In));
        KeyHandler.RegisterKeyDownHandler(Key.OemMinus, () => Viewport?.Zoom(ZoomDirection.Out));

        // height level key
        KeyHandler.RegisterKeyDownHandler(Key.OemPeriod, () => Viewport?.MoveHeightLevel(HeightLevelDirection.Up, 1));
        KeyHandler.RegisterKeyDownHandler(Key.OemComma, () => Viewport?.MoveHeightLevel(HeightLevelDirection.Down, 1));
    }

    private void ConfigureMouseHandler()
    {
        // move camera
        MouseHandler.RegisterHandler(
            mouse => TranslateCameraWithMouse(mouse.MouseDelta),
            condition: mouse => mouse.IsMouseLeft && mouse.MouseDelta != PointY<int>.Zero,
            MouseHandlerWhen.MouseMove);

        // update context world information under mouse cursor when mouse moved
        MouseHandler.RegisterHandler(
            mouse =>
            {
                if (Viewport is null)
                    return;
                // convert mouse screen position to world position
                PointZ<int> worldPos = PointSpaceConversion.ConvertScreenPosToWorldPos(
                    mouse.MousePos.ToFloat(),
                    Viewport.CameraPos,
                    Viewport.UnitMultiplier,
                    Viewport.ScreenSize.ToFloat()).Floor();
                Viewport.UpdateContextWorldInformation(worldPos);
            },
            condition: _ => true,
            MouseHandlerWhen.MouseMove);

        // zoom in/out
        MouseHandler.RegisterHandler(
            mouse =>
            {
                ZoomDirection direction = mouse.ScrollDelta > 0 ? ZoomDirection.In : ZoomDirection.Out;
                Viewport?.Zoom(direction);
            },
            condition: _ => true,
            MouseHandlerWhen.MouseWheel);
    }

    private void TranslateCameraWithKey(PointZ<float> direction)
    {
        if (Viewport is null)
            return;
        Viewport.TranslateCamera(direction * 50 / Viewport.ZoomMultiplier);
    }

    private void TranslateCameraWithMouse(PointY<int> mouseDelta)
    {
        if (Viewport is null)
            return;
        PointZ<float> cameraDelta = -(mouseDelta.ToFloat() / Viewport.UnitMultiplier);
        Viewport.TranslateCamera(cameraDelta);
    }
}
