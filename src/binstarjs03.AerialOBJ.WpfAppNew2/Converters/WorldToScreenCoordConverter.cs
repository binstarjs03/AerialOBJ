using System;
using System.Globalization;
using System.Windows.Data;

using binstarjs03.AerialOBJ.Core.Primitives;
using binstarjs03.AerialOBJ.WpfAppNew2.Components;
using binstarjs03.AerialOBJ.WpfAppNew2.Services;

using Microsoft.Extensions.DependencyInjection;

namespace binstarjs03.AerialOBJ.WpfAppNew2.Converters;
public class WorldToScreenCoordConverter : IValueConverter
{
    private ICoordinateConverterService _converterService;

    public WorldToScreenCoordConverter()
    {
        _converterService = App.Current.Host.Services.GetRequiredService<ICoordinateConverterService>();
    }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        Point2<float> worldPoint = (Point2<float>)value;
        ViewportArg viewportArg = (ViewportArg)parameter;
        return _converterService.ConvertWorldToScreen(worldPoint, viewportArg);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
