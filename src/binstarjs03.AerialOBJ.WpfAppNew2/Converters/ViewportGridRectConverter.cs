using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace binstarjs03.AerialOBJ.WpfAppNew2.Converters;
public class ViewportGridRectConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        try
        {
            float gridSize = (float)values[0];
            float unitMultiplier = (float)values[1];
            float screenGridSize = gridSize * unitMultiplier;
            return new Rect(0, 0, screenGridSize, screenGridSize);
        }
        catch { return DependencyProperty.UnsetValue; }
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
