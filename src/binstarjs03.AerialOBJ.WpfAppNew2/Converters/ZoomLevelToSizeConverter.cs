using System;
using System.Globalization;
using System.Windows.Data;

namespace binstarjs03.AerialOBJ.WpfAppNew2.Converters;
public class ZoomLevelToSizeConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        float absoluteSize = (float)values[0];
        float zoomLevel = (float)values[1];
        return (double)(absoluteSize * zoomLevel);
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
