using System;
using System.Globalization;
using System.Windows.Data;

using binstarjs03.AerialOBJ.Core;

namespace binstarjs03.AerialOBJ.WpfAppNew2.Converters;
public class WorldToScreenCoordConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        float worldPos = (float)values[0];
        float cameraPos = (float)values[1];
        float unitMultiplier = (float)values[2];
        int screenSize = (int)values[3];
        float result = MathUtils.PointSpaceConversion.ConvertWorldPosToScreenPos(worldPos, cameraPos, unitMultiplier, screenSize);
        return (double)result;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
