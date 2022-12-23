using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

using binstarjs03.AerialOBJ.Core;
using binstarjs03.AerialOBJ.Core.Primitives;
using binstarjs03.AerialOBJ.WpfAppNew2.Components;
using binstarjs03.AerialOBJ.WpfAppNew2.Models;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace binstarjs03.AerialOBJ.WpfAppNew2.ViewModels;
[ObservableObject]
public partial class ViewportViewModel : IViewportViewModel
{
    private float _zoomRatio = 1.5f;

    [ObservableProperty] private Size<int> _screenSize = new(1, 1);
    [ObservableProperty] private Point2Z<float> _cameraPos = Point2Z<float>.Zero;
    [ObservableProperty] private float _zoomLevel = 1f;

    [ObservableProperty] private Point2<int> _mousePos = Point2<int>.Zero;
    [ObservableProperty] private Vector2<int> _mousePosDelta = Vector2<int>.Zero;
    [ObservableProperty] private bool _mouseClickHolding = false;
    [ObservableProperty] private bool _mouseInitClickDrag = true;
    [ObservableProperty] private bool _mouseIsOutside = true;

    public ObservableCollection<ViewportObject> ViewportObjects { get; } = new();

    public ViewportViewModel(GlobalState globalState)
    {
        GlobalState = globalState;
        GlobalState.PropertyChanged += OnPropertyChanged;

        Random rnd = new();
        for (int x = 0; x < 5; x++)
            for (int y = 0; y < 5; y++)
            {
                ViewportObjects.Add(new ViewportObject()
                {
                    Size = 16,
                    WorldPosition = new Point2<float>(x * 16, y * 16),
                    Color = new Color()
                    {
                        Red = (byte)rnd.Next(0, 128),
                        Green = (byte)rnd.Next(64, 192),
                        Blue = (byte)rnd.Next(128, 256),
                        Alpha = 255,
                    },
                });
            }
    }

    public GlobalState GlobalState { get; }

    [RelayCommand]
    private void OnScreenSizeChanged(SizeChangedEventArgs e)
    {
        Size newSize = e.NewSize;
        ScreenSize = new Size<int>(newSize.Width.Floor(), newSize.Height.Floor());
    }

    [RelayCommand]
    private void OnMouseMove(MouseEventArgs e)
    {
        Point point = e.GetPosition(e.Source as IInputElement);
        Point2<int> oldMousePos = MousePos;
        Point2<int> newMousePos = new Point2<int>(point.X.Floor(), point.Y.Floor());
        Vector2<int> newMousePosDelta = newMousePos - oldMousePos;
        MousePos = newMousePos;
        MousePosDelta = MouseInitClickDrag && MouseClickHolding ? Vector2<int>.Zero : newMousePosDelta;
        if (MouseClickHolding)
        {
            Vector2Z<float> cameraPosDelta = new(-MousePosDelta.X / ZoomLevel, -MousePosDelta.Y / ZoomLevel);
            CameraPos += cameraPosDelta;
            MouseInitClickDrag = false;
        }
    }

    [RelayCommand]
    private void OnMouseWheel(MouseWheelEventArgs e)
    {
        float newZoomLevel;
        if (e.Delta > 0)
            newZoomLevel = ZoomLevel * _zoomRatio;
        else
            newZoomLevel = ZoomLevel / _zoomRatio;
        // limit zoom scrollability by 8
        newZoomLevel = float.Clamp(newZoomLevel, 1, 1 * MathF.Pow(_zoomRatio, 8));
        ZoomLevel = newZoomLevel;
    }

    [RelayCommand]
    private void OnMouseUp(MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Released)
        {
            MouseClickHolding = false;
            MouseInitClickDrag = true;
        }
    }

    [RelayCommand]
    private void OnMouseDown(MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
            MouseClickHolding = true;
    }

    [RelayCommand]
    private void OnMouseEnter()
    {
        MouseIsOutside = false;
    }

    [RelayCommand]
    private void OnMouseLeave()
    {
        MouseIsOutside = true;
        MouseClickHolding = false;
    }
}
