using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

using binstarjs03.AerialOBJ.Core.Primitives;
using binstarjs03.AerialOBJ.WpfApp.ExtensionMethods;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace binstarjs03.AerialOBJ.WpfApp.Services.Input;

[ObservableObject]
public partial class MouseHandler : IMouseHandler
{
    [ObservableProperty] private PointY<int> _mousePos;
    [ObservableProperty] private PointY<int> _mouseDelta;
    [ObservableProperty] private bool _isMouseInside;
    [ObservableProperty] private bool _isMouseDown;
    [ObservableProperty] private bool _isMouseLeft;
    [ObservableProperty] private bool _isMouseRight;

    private readonly List<RegisteredHandler> _mouseMoveHandler = new();
    private readonly List<RegisteredHandler> _mouseWheelHandler = new();

    public int ScrollDelta { get; private set; }
    private bool InitMouseFocus { get; set; }

    public void RegisterHandler(Action<IMouseHandler> handler, MouseHandlerCondition condition, MouseHandlerWhen when)
    {
        if (when == MouseHandlerWhen.MouseMove)
            _mouseMoveHandler.Add(new RegisteredHandler(handler, condition));
        else if (when == MouseHandlerWhen.MouseWheel)
            _mouseWheelHandler.Add(new RegisteredHandler(handler, condition));
        else
            throw new NotImplementedException();
    }

    [RelayCommand]
    private void OnMouseMove(MouseEventArgs e)
    {
        PointY<int> mousePos = e.GetPosition(e.Source as IInputElement).Floor();
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
    private void OnMouseLeave()
    {
        IsMouseInside = false;
    }

    [RelayCommand]
    private void OnMouseEnter()
    {
        IsMouseInside = true;
    }

    [RelayCommand]
    private void OnMouseDown(MouseButtonEventArgs e)
    {
        IsMouseDown = true;
        IsMouseLeft = e.LeftButton == MouseButtonState.Pressed;
        IsMouseRight = e.RightButton == MouseButtonState.Pressed;
    }

    [RelayCommand]
    private void OnMouseUp(MouseButtonEventArgs e)
    {
        IsMouseDown = false;
        IsMouseLeft = e.LeftButton == MouseButtonState.Pressed;
        IsMouseRight = e.RightButton == MouseButtonState.Pressed;
    }

    [RelayCommand]
    private void OnMouseWheel(MouseWheelEventArgs e)
    {
        ScrollDelta = e.Delta;
        // invoke registered handlers
        foreach (var handler in _mouseWheelHandler)
            if (handler.Condition.Invoke(this))
                handler.Handler.Invoke(this);
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
        public RegisteredHandler(Action<IMouseHandler> handler, MouseHandlerCondition condition)
        {
            Handler = handler;
            Condition = condition;
        }

        public Action<IMouseHandler> Handler { get; }
        public MouseHandlerCondition Condition { get; }
    }
}
