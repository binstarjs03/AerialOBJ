using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace binstarjs03.AerialOBJ.WpfAppNew.Components.Converters;

public class PairToFString : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Point point)
            return $"({Math.Round(point.X, 2)}, {Math.Round(point.Y, 2)})";
        else if (value is Vector vector)
            return $"({Math.Round(vector.X, 2)}, {Math.Round(vector.Y, 2)})";

        else if (value is Size size)
            return $"({Math.Round(size.Width, 2)}, {Math.Round(size.Height, 2)})";
        else
            throw new NotImplementedException();
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
