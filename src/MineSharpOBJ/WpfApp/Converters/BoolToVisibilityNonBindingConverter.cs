using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace binstarjs03.MineSharpOBJ.WpfApp.BindingConverters;

public static class BoolToVisibilityNonBindingConverter
{
    public static Visibility BoolToVisibleCollapsed(bool isVisible)
    {
        if (isVisible)
            return Visibility.Visible;
        else
            return Visibility.Collapsed;
    }

    public static Visibility BoolToVisibleHidden(bool isVisible)
    {
        if (isVisible)
            return Visibility.Visible;
        else
            return Visibility.Hidden;
    }

    public static bool VisibleToBool(Visibility visibility)
    {
        return visibility switch
        {
            Visibility.Visible => true,
            Visibility.Collapsed => false,
            Visibility.Hidden => false,
            _ => throw new Exception("Unknown visibility enum"),
        };
    }
}