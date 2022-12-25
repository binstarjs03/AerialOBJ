using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace binstarjs03.AerialOBJ.WpfAppNew2.Converters;
public class ViewportGridRectConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length != 2)
            throw new ArgumentException("Incorrect number of values. Expected 2");
        if (values[0] is not float || values[1] is not float)
            throw new ArgumentException("Incorrect types for values. " +
                $"Expected {nameof(Single)} and {nameof(Single)}");

        float gridSize = (float)values[0];
        float unitMultiplier = (float)values[1];
        float screenGridSize = gridSize * unitMultiplier;
        return new Rect(0, 0, screenGridSize, screenGridSize);
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
