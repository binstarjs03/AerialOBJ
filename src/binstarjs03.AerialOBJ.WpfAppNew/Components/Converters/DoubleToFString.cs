using System;
using System.Globalization;
using System.Windows.Data;

namespace binstarjs03.AerialOBJ.WpfAppNew.Components.Converters;

public class DoubleToFString : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return $"{Math.Round((double)value, 2)}";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
