using System;
using System.Globalization;
using System.Windows.Data;

namespace binstarjs03.CubeOBJ.WpfApp.Converters;

public class DoubleToString : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return ((double)value).ToString();
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        string str = (string)value;
        if (string.IsNullOrEmpty(str))
            return 0;
        str = str.Replace('.', ',');
        if (str.StartsWith(','))
            str = "0" + str;
        return double.Parse(str);
    }
}
