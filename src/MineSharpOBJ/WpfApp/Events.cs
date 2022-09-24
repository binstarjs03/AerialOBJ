using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace binstarjs03.MineSharpOBJ.WpfApp;

public static class Events
{
    public delegate void VisibilityChanged(object sender, Visibility visibility);
}

public class VisibilityChangedEventArgs : RoutedEventArgs
{
    public VisibilityChangedEventArgs(RoutedEvent routedEvent, Visibility visibility) : base(routedEvent)
    {
        Visibility = visibility;
    }

    public Visibility Visibility { get; }
}