using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

using binstarjs03.AerialOBJ.Core.Primitives;
using binstarjs03.AerialOBJ.MvvmAppCore.Services.Input;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace binstarjs03.AerialOBJ.WpfApp.Services.Input;
public partial class Mouse : ObservableObject, IMouse
{
    [ObservableProperty] private PointY<int> _mousePos;
    [ObservableProperty] private PointY<int> _mouseDelta;
    [ObservableProperty] private bool _isMouseInside;
    [ObservableProperty] private bool _isMouseDown;
    [ObservableProperty] private bool _isMouseLeft;
    [ObservableProperty] private bool _isMouseRight;

    private readonly List<RegisteredHandler> _mouseMoveHandler = new();
    private readonly List<RegisteredHandler> _mouseWheelHandler = new();
    private readonly List<RegisteredHandler> _mouseDownHandler = new();

    public int ScrollDelta { get; private set; }
    private bool InitMouseFocus { get; set; }

    public void RegisterHandler(Action<IMouse> handler, MouseHandlingCondition condition, MouseWhen when)
    {
        switch (when)
        {
            case MouseWhen.MouseMove:
                _mouseMoveHandler.Add(new RegisteredHandler(handler, condition));
                break;
            case MouseWhen.MouseWheel:
                _mouseWheelHandler.Add(new RegisteredHandler(handler, condition));
                break;
            case MouseWhen.MouseDown:
                _mouseDownHandler.Add(new RegisteredHandler(handler, condition));
                break;
            default:
                throw new NotImplementedException("Conforming YAGNI, implement when needed");
        }
    }

    [RelayCommand]
    private void OnMouseMove(object e)
    {
        var arg = CheckAndCast<MouseEventArgs>(e);
        PointY<int> mousePos = arg.GetPosition(arg.Source as IInputElement).Floor();
        MouseDelta = InitMouseFocus ? new PointY<int>() : mousePos - MousePos;
        if (InitMouseFocus)
            InitMouseFocus = false;
        MousePos = mousePos;

        // invoke registered handlers
        foreach (var handler in _mouseMoveHandler)
            if (handler.Condition.Invoke(this))
                handler.Handler.Invoke(this);
    }

    [RelayCommand]
    private void OnMouseLeave(object e) => IsMouseInside = false;

    [RelayCommand]
    private void OnMouseEnter(object e) => IsMouseInside = true;

    [RelayCommand]
    private void OnMouseDown(object e)
    {
        var arg = CheckAndCast<MouseButtonEventArgs>(e);
        IsMouseLeft = arg.LeftButton == MouseButtonState.Pressed;
        IsMouseRight = arg.RightButton == MouseButtonState.Pressed;
        foreach (var handler in _mouseDownHandler)
            if (handler.Condition.Invoke(this))
                handler.Handler.Invoke(this);
    }

    [RelayCommand]
    private void OnMouseUp(object e)
    {
        var arg = CheckAndCast<MouseButtonEventArgs>(e);
        IsMouseDown = false;
        IsMouseLeft = arg.LeftButton == MouseButtonState.Pressed;
        IsMouseRight = arg.RightButton == MouseButtonState.Pressed;
    }

    [RelayCommand]
    private void OnMouseWheel(object e)
    {
        var arg = CheckAndCast<MouseWheelEventArgs>(e);
        ScrollDelta = arg.Delta;
        // invoke registered handlers
        foreach (var handler in _mouseWheelHandler)
            if (handler.Condition.Invoke(this))
                handler.Handler.Invoke(this);
    }

    private T CheckAndCast<T>(object e) where T : class
    {
        if (e is not T arg)
            throw new ArgumentException($"Argument must be typeof {nameof(T)}", nameof(e));
        return arg;
    }

    partial void OnIsMouseInsideChanged(bool value)
    {
        if (value)
            return;
        InitMouseFocus = true;
        IsMouseDown = false;
        IsMouseLeft = false;
        IsMouseRight = false;
    }

    // only relevant for this class, encapsulate it as private
    private class RegisteredHandler
    {
        public RegisteredHandler(Action<IMouse> handler, MouseHandlingCondition condition)
        {
            Handler = handler;
            Condition = condition;
        }

        public Action<IMouse> Handler { get; }
        public MouseHandlingCondition Condition { get; }
    }
}
