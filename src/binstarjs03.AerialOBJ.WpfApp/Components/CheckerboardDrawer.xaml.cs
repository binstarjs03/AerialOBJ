using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace binstarjs03.AerialOBJ.WpfApp.Components;
public partial class CheckerboardDrawer : UserControl
{
    public CheckerboardDrawer()
    {
        InitializeComponent();
    }

    public int TileWidth
    {
        get { return (int)GetValue(TileWidthProperty); }
        set { SetValue(TileWidthProperty, value); }
    }

    public static readonly DependencyProperty TileWidthProperty =
        DependencyProperty.Register(nameof(TileWidth),
                                    typeof(int),
                                    typeof(CheckerboardDrawer),
                                    new PropertyMetadata(8));

    public int TileHeight
    {
        get { return (int)GetValue(TileHeightProperty); }
        set { SetValue(TileHeightProperty, value); }
    }

    public static readonly DependencyProperty TileHeightProperty =
        DependencyProperty.Register(nameof(TileHeight),
                                    typeof(int),
                                    typeof(CheckerboardDrawer),
                                    new PropertyMetadata(8));

    public Brush Color1
    {
        get { return (Brush)GetValue(Color1Property); }
        set { SetValue(Color1Property, value); }
    }

    public static readonly DependencyProperty Color1Property =
        DependencyProperty.Register(nameof(Color1),
                                    typeof(Brush),
                                    typeof(CheckerboardDrawer),
                                    new PropertyMetadata(new SolidColorBrush()));

    public Brush Color2
    {
        get { return (Brush)GetValue(Color2Property); }
        set { SetValue(Color2Property, value); }
    }

    public static readonly DependencyProperty Color2Property =
        DependencyProperty.Register(nameof(Color2),
                                    typeof(Brush),
                                    typeof(CheckerboardDrawer),
                                    new PropertyMetadata(new SolidColorBrush()));
}

public class CheckerboardTileSizeConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        try
        {
            int width = System.Convert.ToInt32(values[0]);
            int height = System.Convert.ToInt32(values[1]);
            return new Rect(0, 0, width, height);
        }
        catch { return DependencyProperty.UnsetValue; }
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
