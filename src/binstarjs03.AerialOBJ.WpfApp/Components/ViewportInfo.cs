using System.Windows;

using binstarjs03.AerialOBJ.Core.Primitives;

namespace binstarjs03.AerialOBJ.WpfApp.Components;

public class ViewportInfo : DependencyObject
{
    public static readonly DependencyProperty CameraPosProperty =
        DependencyProperty.RegisterAttached("CameraPos",
                                            typeof(PointZ<float>),
                                            typeof(ViewportInfo),
                                            new PropertyMetadata(PointZ<float>.Zero));

    public static PointZ<float> GetCameraPos(DependencyObject d) =>
        (PointZ<float>)d.GetValue(CameraPosProperty);

    public static void SetCameraPos(DependencyObject d, PointZ<float> value) =>
        d.SetValue(CameraPosProperty, value);

    public static readonly DependencyProperty ScreenSizeProperty =
        DependencyProperty.RegisterAttached("ScreenSize",
                                            typeof(Size<int>),
                                            typeof(ViewportInfo),
                                            new PropertyMetadata(new Size<int>()));

    public static Size<int> GetScreenSize(DependencyObject d) =>
        (Size<int>)d.GetValue(ScreenSizeProperty);

    public static void SetScreenSize(DependencyObject d, Size<int> value) =>
        d.SetValue(ScreenSizeProperty, value);

    public static readonly DependencyProperty ZoomMultiplierProperty =
        DependencyProperty.RegisterAttached("ZoomMultiplier",
                                            typeof(float),
                                            typeof(ViewportInfo),
                                            new PropertyMetadata(0f));

    public static float GetZoomMultiplier(DependencyObject d) =>
        (float)d.GetValue(ZoomMultiplierProperty);

    public static void SetZoomMultiplier(DependencyObject d, float value) =>
        d.SetValue(ZoomMultiplierProperty, value);
}