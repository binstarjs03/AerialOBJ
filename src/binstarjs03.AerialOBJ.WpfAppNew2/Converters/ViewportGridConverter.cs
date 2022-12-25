using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

using binstarjs03.AerialOBJ.Core.Primitives;
using binstarjs03.AerialOBJ.WpfAppNew2.Services;

namespace binstarjs03.AerialOBJ.WpfAppNew2.Converters;
public class ViewportGridConverter : IMultiValueConverter
{
    private readonly ICoordinateConverterService _coordsConverterService;

    public ViewportGridConverter()
    {
        _coordsConverterService = new CoordinateConverterService();
    }

    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length != 4)
            throw new ArgumentException("Incorrect number of values. Expected 4");
        if (values[0] is not float || values[1] is not Point2Z<float> || values[2] is not float || values[3] is not Size<int>)
            throw new ArgumentException("Incorrect types for values. " +
                $"Expected {nameof(Single)}, {nameof(Point2Z<float>)}, {nameof(Single)}, and {nameof(Size<int>)}");

        float gridSize = (float)values[0];
        Point2Z<float> cameraPos = (Point2Z<float>)values[1];
        float unitMultiplier = (float)values[2];
        Size<int> screenSize = (Size<int>)values[3];

        Point2Z<float> worldPos = new(0f, 0f);
        Point2<float> screenPos = _coordsConverterService.ConvertWorldToScreen(worldPos, cameraPos, unitMultiplier, screenSize);
        float screenGridSize = gridSize * unitMultiplier;
        return new Rect(screenPos.X, screenPos.Y, screenGridSize, screenGridSize);
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
