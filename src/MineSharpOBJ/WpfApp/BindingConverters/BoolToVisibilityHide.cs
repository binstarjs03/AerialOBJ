using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace binstarjs03.MineSharpOBJ.WpfApp.BindingConverters;

public class BoolToVisibilityHide : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        bool isTrue = (bool)value;
        if (isTrue)
            return Visibility.Visible;
        else
            return Visibility.Hidden;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        Visibility visibility = (Visibility)value;
        return visibility switch {
            Visibility.Visible => true,
            Visibility.Hidden => false,
            _ => throw new Exception($"{nameof(BoolToVisibilityHide)} convert back not returning visible or hidden"),
        };
    }
}