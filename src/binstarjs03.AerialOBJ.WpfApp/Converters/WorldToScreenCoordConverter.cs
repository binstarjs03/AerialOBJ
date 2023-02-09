using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

using binstarjs03.AerialOBJ.Core;
using binstarjs03.AerialOBJ.Core.Primitives;

namespace binstarjs03.AerialOBJ.WpfApp.Converters;
public class WorldToScreenCoordConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        try
        {
            float worldPos = System.Convert.ToSingle(values[0]);
            float cameraPos = System.Convert.ToSingle(values[1]);
            float unitMultiplier = System.Convert.ToSingle(values[2]);
            int screenSize = System.Convert.ToInt32(values[3]);
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
