using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

using binstarjs03.MineSharpOBJ.WpfApp.BindingConverters;
using binstarjs03.MineSharpOBJ.WpfApp.Services;

namespace binstarjs03.MineSharpOBJ.WpfApp.UIElements.Windows;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        MainWindowVM vm = new(this);
        DataContext = vm;

        DebugLogWindow = new DebugLogWindow(this);
        Show();
        DebugLogWindow.Owner = this;

        MainService.Initialize();
    }

    // Accessors --------------------------------------------------------------

    public DebugLogWindow DebugLogWindow { get; }

    // Events -----------------------------------------------------------------

    private void DebugLogWindow_SynchronizePosition()
    {
        DebugLogWindow.Top = Top;
        DebugLogWindow.Left = Left + ActualWidth;
    }

    protected override void OnLocationChanged(EventArgs e)
    {
        DebugLogWindow_SynchronizePosition();
    }

    protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
    {
        DebugLogWindow_SynchronizePosition();
    }

    //public static readonly RoutedEvent DebugLogWindowVisibilityChangedEvent
    //    = EventManager.RegisterRoutedEvent
    //(
    //    nameof(DebugLogWindowVisibilityChanged),
    //    RoutingStrategy.Bubble,
    //    typeof(RoutedEventHandler),
    //    typeof(MainWindow)
    //);
    //public event RoutedEventHandler DebugLogWindowVisibilityChanged
    //{
    //    add { AddHandler(DebugLogWindowVisibilityChangedEvent, value); }
    //    remove { RemoveHandler(DebugLogWindowVisibilityChangedEvent, value); }
    //}
}
