using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace binstarjs03.AerialOBJ.WpfApp.Converters;
public class FlexibleBoolToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        try
        {
            return VisibilityFromParameter(parameter.ToString()!, (bool)value);
        }
        catch { return DependencyProperty.UnsetValue; }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        Visibility visibility = (Visibility)value;
        Visibility trueVisibility = VisibilityFromParameter(parameter.ToString()!, true);
        return visibility == trueVisibility;
    }

    private static Visibility VisibilityFromParameter(string parameter, bool value)
    {
        string[] visibilities = parameter.Split("|");
        string visibility = value ? visibilities[0] : visibilities[1];
        return (Visibility)Enum.Parse(typeof(Visibility), visibility);
    }
}
