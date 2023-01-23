using System;

using binstarjs03.AerialOBJ.Core.Primitives;

namespace binstarjs03.AerialOBJ.WpfApp.Services.Input;

public delegate bool MouseHandlerCondition(IMouseHandler mouse);

public interface IMouseHandler
{
    public PointY<int> MousePos { get; set; }
    public PointY<int> MouseDelta { get; }
    public bool IsMouseInside { get; }
    public bool IsMouseDown { get; }
    public bool IsMouseLeft { get; }
    public bool IsMouseRight { get; }
    public int ScrollDelta { get; }

    void RegisterHandler(Action<IMouseHandler> handler, MouseHandlerCondition condition, MouseHandlerWhen when);
}

public enum MouseHandlerWhen
{
    MouseMove,
    MouseLeave,
    MouseEnter,
    MouseDown,
    MouseUp,
    MouseWheel
}