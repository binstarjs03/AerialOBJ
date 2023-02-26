using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using binstarjs03.AerialOBJ.Core.Primitives;

namespace binstarjs03.AerialOBJ.WpfApp.Components;
public partial class GridDrawer : UserControl
{
    public GridDrawer()
    {
        InitializeComponent();
    }

    public float GridSize
    {
        get { return (float)GetValue(GridSizeProperty); }
        set { SetValue(GridSizeProperty, value); }
    }

    public static readonly DependencyProperty GridSizeProperty =
        DependencyProperty.Register(nameof(GridSize), typeof(float), typeof(GridDrawer), new PropertyMetadata(0f));

    public SolidColorBrush GridColor
    {
        get { return (SolidColorBrush)GetValue(GridColorProperty); }
        set { SetValue(GridColorProperty, value); }
    }

    public static readonly DependencyProperty GridColorProperty =
        DependencyProperty.Register(nameof(GridColor), typeof(SolidColorBrush), typeof(GridDrawer), new PropertyMetadata(new SolidColorBrush()));

    public double GridThickness
    {
        get { return (double)GetValue(GridThicknessProperty); }
        set { SetValue(GridThicknessProperty, value); }
    }

    public static readonly DependencyProperty GridThicknessProperty =
        DependencyProperty.Register(nameof(GridThickness), typeof(double), typeof(GridDrawer), new PropertyMetadata(0d));
}
