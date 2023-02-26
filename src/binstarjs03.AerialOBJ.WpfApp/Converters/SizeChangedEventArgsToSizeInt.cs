using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

using binstarjs03.AerialOBJ.Core;
using binstarjs03.AerialOBJ.Core.Primitives;

namespace binstarjs03.AerialOBJ.WpfApp.Converters;
public class SizeChangedEventArgsToSizeInt : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not SizeChangedEventArgs arg)
            return DependencyProperty.UnsetValue;
        var newsize = arg.NewSize;
        return new Size<int>(newsize.Width.Floor(), newsize.Height.Floor());
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
