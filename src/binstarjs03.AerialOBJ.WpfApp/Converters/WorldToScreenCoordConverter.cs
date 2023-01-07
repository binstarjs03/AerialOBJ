using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

using binstarjs03.AerialOBJ.Core;
using binstarjs03.AerialOBJ.WpfApp.ExtensionMethods;

namespace binstarjs03.AerialOBJ.WpfApp.Converters;
public class WorldToScreenCoordConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        try
        {
            float worldPos = (float)values[0];
            float cameraPos = (float)values[1];
            float unitMultiplier = (float)values[2];
            int screenSize = (int)values[3];
            float result = PointSpaceConversion.ConvertWorldPosToScreenPos(worldPos, cameraPos, unitMultiplier, screenSize);
            return (double)result.Floor();
        }
        catch { return DependencyProperty.UnsetValue; }
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
