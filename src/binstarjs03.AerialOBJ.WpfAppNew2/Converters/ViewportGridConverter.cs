using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

using binstarjs03.AerialOBJ.Core;
using binstarjs03.AerialOBJ.Core.Primitives;

namespace binstarjs03.AerialOBJ.WpfAppNew2.Converters;
public class ViewportGridConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        try
        {
            float gridSize = (float)values[0];
            Point2Z<float> cameraPos = (Point2Z<float>)values[1];
            float unitMultiplier = (float)values[2];
            Size<int> screenSize = (Size<int>)values[3];

            Point2Z<float> worldPos = new(0f, 0f);
            Point2<float> screenPos = MathUtils.PointSpaceConversion.ConvertWorldPosToScreenPos(worldPos, cameraPos, unitMultiplier, screenSize);
            float screenGridSize = gridSize * unitMultiplier;
            return new Rect(screenPos.X, screenPos.Y, screenGridSize, screenGridSize);
        }
        catch { return DependencyProperty.UnsetValue; }
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
