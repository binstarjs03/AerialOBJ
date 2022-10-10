using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace binstarjs03.CubeOBJ.WpfApp.Converters;

public class BoolToVisibility : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool isTrue = (bool)value;
        if (isTrue)
            return Visibility.Visible;
        else
            return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        Visibility visibility = (Visibility)value;
        return visibility switch
        {
            Visibility.Visible => true,
            Visibility.Collapsed => false,
            _ => throw new Exception($"{nameof(BoolToVisibility)} convert back not set from visible or collapsed"),
        };
    }
}
