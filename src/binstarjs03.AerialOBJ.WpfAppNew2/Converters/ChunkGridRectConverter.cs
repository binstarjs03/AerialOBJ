using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

using binstarjs03.AerialOBJ.Core.MinecraftWorld;

namespace binstarjs03.AerialOBJ.WpfAppNew2.Converters;
public class ChunkGridRectConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        float unitMultiplier = (float)value;
        float gridSize = unitMultiplier * Section.BlockCount;
        return new Rect(0,0, gridSize, gridSize);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
