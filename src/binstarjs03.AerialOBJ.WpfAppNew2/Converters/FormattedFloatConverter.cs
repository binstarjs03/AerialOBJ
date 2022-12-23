using System;
using System.Globalization;
using System.Windows.Data;

using binstarjs03.AerialOBJ.Core;
using binstarjs03.AerialOBJ.Core.Primitives;

namespace binstarjs03.AerialOBJ.WpfAppNew2.Converters;
public class FormattedFloatConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Point2Z<float> point)
            return $"({point.X.Round()}, {point.Z.Round()})";
        else 
            throw new NotImplementedException();
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
