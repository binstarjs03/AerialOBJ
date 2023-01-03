using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using binstarjs03.AerialOBJ.Core.Primitives;

namespace binstarjs03.AerialOBJ.WpfApp.Views.ReusableControls;
public partial class GridDrawerView : UserControl
{
    public GridDrawerView()
    {
        InitializeComponent();
    }

    public float GridSize
    {
        get { return (float)GetValue(GridSizeProperty); }
        set { SetValue(GridSizeProperty, value); }
    }

    public static readonly DependencyProperty GridSizeProperty =
        DependencyProperty.Register(nameof(GridSize), typeof(float), typeof(GridDrawerView), new PropertyMetadata(0f));

    public Point2Z<float> CameraPos
    {
        get { return (Point2Z<float>)GetValue(CameraPosProperty); }
        set { SetValue(CameraPosProperty, value); }
    }

    public static readonly DependencyProperty CameraPosProperty =
        DependencyProperty.Register(nameof(CameraPos), typeof(Point2Z<float>), typeof(GridDrawerView), new PropertyMetadata(new Point2Z<float>(0,0)));

    public float UnitMultiplier
    {
        get { return (float)GetValue(UnitMultiplierProperty); }
        set { SetValue(UnitMultiplierProperty, value); }
    }

    public static readonly DependencyProperty UnitMultiplierProperty =
        DependencyProperty.Register(nameof(UnitMultiplier), typeof(float), typeof(GridDrawerView), new PropertyMetadata(0f));

    public Size<int> ScreenSize
    {
        get { return (Size<int>)GetValue(ScreenSizeProperty); }
        set { SetValue(ScreenSizeProperty, value); }
    }

    public static readonly DependencyProperty ScreenSizeProperty =
        DependencyProperty.Register(nameof(ScreenSize), typeof(Size<int>), typeof(GridDrawerView), new PropertyMetadata(new Size<int>(1,1)));

    public SolidColorBrush GridColor
    {
        get { return (SolidColorBrush)GetValue(GridColorProperty); }
        set { SetValue(GridColorProperty, value); }
    }

    public static readonly DependencyProperty GridColorProperty =
        DependencyProperty.Register(nameof(GridColor), typeof(SolidColorBrush), typeof(GridDrawerView), new PropertyMetadata(new SolidColorBrush()));

    public double GridThickness
    {
        get { return (double)GetValue(GridThicknessProperty); }
        set { SetValue(GridThicknessProperty, value); }
    }

    public static readonly DependencyProperty GridThicknessProperty =
        DependencyProperty.Register(nameof(GridThickness), typeof(double), typeof(GridDrawerView), new PropertyMetadata(0d));
}
