using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

using binstarjs03.AerialOBJ.Core.MinecraftWorld;
using binstarjs03.AerialOBJ.Core.Primitives;
using binstarjs03.AerialOBJ.WpfAppNew2.Services;

namespace binstarjs03.AerialOBJ.WpfAppNew2.Converters;
public class ChunkGridViewportConverter : IMultiValueConverter
{
    private readonly ICoordinateConverterService _coordsConverterService;

    public ChunkGridViewportConverter()
    {
        _coordsConverterService = new CoordinateConverterService();
    }

    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (_coordsConverterService is null)
            return new Rect(0, 0, 16, 16);
        Point2Z<float> worldPos = new(0f, 0f);
        Point2Z<float> cameraPos = (Point2Z<float>)values[0];
        float zoomLevel = (float)values[1];
        Size<int> screenSize = (Size<int>)values[2];
        Point2<float> screenPos = _coordsConverterService.ConvertWorldToScreen(worldPos, cameraPos, zoomLevel, screenSize);
        float gridSize = zoomLevel * Section.BlockCount;
        return new Rect(screenPos.X, screenPos.Y, gridSize, gridSize);
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
