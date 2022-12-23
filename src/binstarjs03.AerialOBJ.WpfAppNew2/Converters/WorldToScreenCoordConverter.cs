using System;
using System.Globalization;
using System.Windows.Data;

using binstarjs03.AerialOBJ.WpfAppNew2.Services;

using Microsoft.Extensions.DependencyInjection;

namespace binstarjs03.AerialOBJ.WpfAppNew2.Converters;
public class WorldToScreenCoordConverter : IMultiValueConverter
{
    private ICoordinateConverterService _converterService;

    public WorldToScreenCoordConverter()
    {
        // null-coalesence operator is to fix null error in the designer
        // while the null-forgiving operator we know it wont be null at runtime
        _converterService = App.Current?.Host.Services.GetRequiredService<ICoordinateConverterService>()!;
    }

    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        float worldPos = (float)values[0];
        float cameraPos = (float)values[1];
        float zoomLevel = (float)values[2];
        int screenSize = (int)values[3];
        float result = _converterService.ConvertWorldToScreen(worldPos, cameraPos, zoomLevel, screenSize);
        return (double)result;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
