using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

using binstarjs03.AerialOBJ.Core;
using binstarjs03.AerialOBJ.Core.Primitives;

namespace binstarjs03.AerialOBJ.WpfApp.Converters;
public class ViewportGridConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        try
        {
            float gridSize = (float)values[0];
            PointZ<float> cameraPos = (PointZ<float>)values[1];
            float unitMultiplier = (float)values[2];
            Size<int> screenSize = (Size<int>)values[3];

            PointZ<float> worldPos = new(0f, 0f);
            Size<float> floatScreenSize = new(screenSize.Width, screenSize.Height);
            PointY<float> screenPos = PointSpaceConversion.ConvertWorldPosToScreenPos(worldPos, cameraPos, unitMultiplier, floatScreenSize);
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
