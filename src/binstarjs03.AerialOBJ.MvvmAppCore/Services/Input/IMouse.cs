using System;

using binstarjs03.AerialOBJ.Core.Primitives;

using CommunityToolkit.Mvvm.Input;

namespace binstarjs03.AerialOBJ.MvvmAppCore.Services.Input;
public interface IMouse
{
    PointY<int> MousePos { get; set; }
    PointY<int> MouseDelta { get; }
    bool IsMouseInside { get; }
    bool IsMouseDown { get; }
    bool IsMouseLeft { get; }
    bool IsMouseRight { get; }
    int ScrollDelta { get; }

    IRelayCommand<object?> MouseMoveCommand { get; }
    IRelayCommand<object?> MouseWheelCommand { get; }
    IRelayCommand<object?> MouseUpCommand { get; }
    IRelayCommand<object?> MouseDownCommand { get; }
    IRelayCommand<object?> MouseEnterCommand { get; }
    IRelayCommand<object?> MouseLeaveCommand { get; }

    void RegisterHandler(Action<IMouse> handler, MouseHandlingCondition condition, MouseWhen when);
}
