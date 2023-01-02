using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace binstarjs03.AerialOBJ.WpfApp.Converters;
public class UnitMultiplierConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        try
        {
            float absoluteSize;
            float unitMultiplier = (float)values[1];

            Type sizeType = values[0].GetType();
            if (sizeType == typeof(float))
                absoluteSize = (float)values[0];
            else if (sizeType == typeof(int))
                absoluteSize = (int)values[0];
            else
                return DependencyProperty.UnsetValue;
            return (double)(absoluteSize * unitMultiplier);
        }
        catch { return DependencyProperty.UnsetValue; }
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
