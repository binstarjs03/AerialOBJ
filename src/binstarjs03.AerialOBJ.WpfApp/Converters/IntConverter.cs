using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace binstarjs03.AerialOBJ.WpfApp.Converters;
internal class IntConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not int intvalue)
            return DependencyProperty.UnsetValue;
        return intvalue.ToString();
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not string str)
            return DependencyProperty.UnsetValue;
        if (string.IsNullOrEmpty(str))
            return 0;
        return int.Parse(str);
    }
}
