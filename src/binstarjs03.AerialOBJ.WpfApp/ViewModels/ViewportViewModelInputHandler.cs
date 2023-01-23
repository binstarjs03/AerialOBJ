using System.Windows.Input;

using binstarjs03.AerialOBJ.Core.Primitives;
using binstarjs03.AerialOBJ.WpfApp.Services.Input;

namespace binstarjs03.AerialOBJ.WpfApp.ViewModels;
public class ViewportViewModelInputHandler
{
    public ViewportViewModelInputHandler(IKeyHandler keyHandler)
    {
        KeyHandler = keyHandler;
        ConfigureKeyHandler();
    }

    public IViewportViewModel? Context { get; set; }
    public IKeyHandler KeyHandler { get; }

    private void ConfigureKeyHandler()
    {
        // directional key
        KeyHandler.RegisterKeyDownHandler(Key.Left, () => TranslateCamera(PointZ<float>.Left));
        KeyHandler.RegisterKeyDownHandler(Key.Up, () => TranslateCamera(PointZ<float>.Front));
        KeyHandler.RegisterKeyDownHandler(Key.Right, () => TranslateCamera(PointZ<float>.Right));
        KeyHandler.RegisterKeyDownHandler(Key.Down, () => TranslateCamera(PointZ<float>.Back));

        // zoom key
        KeyHandler.RegisterKeyDownHandler(Key.OemPlus, () => Context?.Zoom(ZoomDirection.In));
        KeyHandler.RegisterKeyDownHandler(Key.OemMinus, () => Context?.Zoom(ZoomDirection.Out));

        // height level key
        KeyHandler.RegisterKeyDownHandler(Key.OemPeriod, () => Context?.MoveHeightLevel(HeightLevelDirection.Up, 1));
        KeyHandler.RegisterKeyDownHandler(Key.OemComma, () => Context?.MoveHeightLevel(HeightLevelDirection.Down, 1));
    }

    private void TranslateCamera(PointZ<float> direction)
    {
        if (Context is null)
            return;
        Context.TranslateCamera(direction * 50 / Context.ZoomMultiplier);
    }
}
