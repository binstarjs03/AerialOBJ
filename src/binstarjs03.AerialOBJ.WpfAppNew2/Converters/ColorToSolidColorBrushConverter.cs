using System;
using System.Globalization;
using System.Windows.Data;
using WPFColor = System.Windows.Media.Color;

using CoreColor = binstarjs03.AerialOBJ.Core.Primitives.Color;
using System.Windows.Media;

namespace binstarjs03.AerialOBJ.WpfAppNew2.Converters;
public class ColorToSolidColorBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        CoreColor coreColor = (CoreColor)value;
        WPFColor wpfColor = WPFColor.FromArgb(coreColor.Alpha, coreColor.Red, coreColor.Green, coreColor.Blue);
        return new SolidColorBrush(wpfColor);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
