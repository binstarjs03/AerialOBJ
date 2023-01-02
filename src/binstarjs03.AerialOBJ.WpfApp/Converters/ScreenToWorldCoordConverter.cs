using System;
using System.Globalization;
using System.Windows.Data;

using binstarjs03.AerialOBJ.Core.Primitives;

using PointSpaceConversion = binstarjs03.AerialOBJ.Core.MathUtils.PointSpaceConversion;

namespace binstarjs03.AerialOBJ.WpfApp.Converters;
public class ScreenToWorldCoordConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        Point2<int> screenPos = (Point2<int>)values[0];
        Point2Z<float> cameraPos = (Point2Z<float>)values[1];
        Size<int> screenSize = (Size<int>)values[2];
        float unitMultiplier = (float)values[3];

        float worldPosX = PointSpaceConversion.ConvertScreenPosToWorldPos(screenPos.X, cameraPos.X, unitMultiplier, screenSize.Width);
        float worldPosZ = PointSpaceConversion.ConvertScreenPosToWorldPos(screenPos.Y, cameraPos.Z, unitMultiplier, screenSize.Height);
        return new Point2Z<float>(worldPosX, worldPosZ);
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
