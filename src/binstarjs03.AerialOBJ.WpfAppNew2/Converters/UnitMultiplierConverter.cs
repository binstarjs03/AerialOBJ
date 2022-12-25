using System;
using System.Globalization;
using System.Windows.Data;

namespace binstarjs03.AerialOBJ.WpfAppNew2.Converters;
public class UnitMultiplierConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        float absoluteSize;
        float unitMultiplier = (float)values[1];

        Type sizeType = values[0].GetType();
        if (sizeType == typeof(float))
             absoluteSize = (float)values[0];
        else if (sizeType == typeof(int))
            absoluteSize = (int)values[0];
        else
            throw new InvalidCastException();

        return (double)(absoluteSize * unitMultiplier);
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
