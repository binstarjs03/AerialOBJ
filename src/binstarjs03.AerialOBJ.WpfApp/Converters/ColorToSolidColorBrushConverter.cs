using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

using CoreColor = binstarjs03.AerialOBJ.Core.Primitives.Color;
using WPFColor = System.Windows.Media.Color;

namespace binstarjs03.AerialOBJ.WpfApp.Converters;
public class ColorToSolidColorBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        try
        {
            CoreColor coreColor = (CoreColor)value;
            WPFColor wpfColor = WPFColor.FromArgb(coreColor.Alpha, coreColor.Red, coreColor.Green, coreColor.Blue);
            return new SolidColorBrush(wpfColor);
        }
        catch { return DependencyProperty.UnsetValue; }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
