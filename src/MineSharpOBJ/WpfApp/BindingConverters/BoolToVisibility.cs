﻿using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows;
namespace binstarjs03.MineSharpOBJ.WpfApp.BindingConverters;

public class BoolToVisibility : IValueConverter {
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
            _ => false,
        };
    }
}