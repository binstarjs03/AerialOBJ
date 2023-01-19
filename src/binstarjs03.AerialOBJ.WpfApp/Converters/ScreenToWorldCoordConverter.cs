using System;
using System.Globalization;
using System.Windows.Data;

using binstarjs03.AerialOBJ.Core;
using binstarjs03.AerialOBJ.Core.Primitives;

namespace binstarjs03.AerialOBJ.WpfApp.Converters;
public class ScreenToWorldCoordConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        PointY<int> screenPos = (PointY<int>)values[0];
        PointZ<float> cameraPos = (PointZ<float>)values[1];
        Size<int> screenSize = (Size<int>)values[2];
        float unitMultiplier = (float)values[3];

        float worldPosX = PointSpaceConversion.ConvertScreenPosToWorldPos(screenPos.X, cameraPos.X, unitMultiplier, screenSize.Width);
        float worldPosZ = PointSpaceConversion.ConvertScreenPosToWorldPos(screenPos.Y, cameraPos.Z, unitMultiplier, screenSize.Height);
        return new PointZ<float>(worldPosX, worldPosZ);
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
