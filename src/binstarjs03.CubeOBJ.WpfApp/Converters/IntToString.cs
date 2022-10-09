using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace binstarjs03.CubeOBJ.WpfApp.Converters;

public class IntToString : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return ((int)value).ToString();
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        string str = (string)value;
        if (string.IsNullOrEmpty(str))
            return 0;
        return int.Parse(str);
    }
}
