using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace binstarjs03.AerialOBJ.WpfAppNew2.Converters;

public class BoolToVisibility : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool isTrue = (bool)value;
        return isTrue ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        Visibility visibility = (Visibility)value;
        return visibility switch
        {
            Visibility.Visible => true,
            Visibility.Hidden => false,
            Visibility.Collapsed => false,
            _ => throw new ConverterException($"{nameof(BoolToVisibility)} convert back not set from visible, hidden, or collapsed"),
        };
    }
}
