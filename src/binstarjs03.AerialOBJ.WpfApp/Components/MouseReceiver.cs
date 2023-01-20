using System;
using System.Windows;
using System.Windows.Input;

using binstarjs03.AerialOBJ.Core;
using binstarjs03.AerialOBJ.Core.Primitives;
using binstarjs03.AerialOBJ.WpfApp.ExtensionMethods;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace binstarjs03.AerialOBJ.WpfApp.Components;

public delegate void MouseMoveHandler(PointY<int> mousePos, PointY<int> mouseDelta);

[ObservableObject]
public partial class MouseReceiver
{
    [ObservableProperty] private PointY<int> _mousePos;
    [ObservableProperty] private PointY<int> _mouseDelta;
    [ObservableProperty] private bool _isMouseInside;
    [ObservableProperty] private bool _isMouseDown;
    [ObservableProperty] private bool _initMouseFocus;
    [ObservableProperty] private bool _isMouseLeft;
    [ObservableProperty] private bool _isMouseRight;

    public event MouseMoveHandler? MouseMove;
    public event Action<int>? MouseWheel;

    [RelayCommand]
    private void OnMouseMove(MouseEventArgs e)
    {
        PointY<int> mousePos = e.GetPosition(e.Source as IInputElement).Floor();
        MouseDelta = InitMouseFocus ? new PointY<int>() : mousePos - MousePos;
        if (InitMouseFocus)
            InitMouseFocus = false;
        MousePos = mousePos;
        MouseMove?.Invoke(MousePos, MouseDelta);
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
        MouseWheel?.Invoke(e.Delta);
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
}
