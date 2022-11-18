using System;
using System.Windows;

using binstarjs03.AerialOBJ.WpfAppNew.Components.Interfaces;

namespace binstarjs03.AerialOBJ.WpfAppNew.View;

public partial class MainWindow : Window, IClosable
{
    public event RequestSynchronizeWindowPositionEventHandler? RequestSynchronizeWindowPosition;
    public delegate void RequestSynchronizeWindowPositionEventHandler(double top, double left);

    public MainWindow()
    {
        InitializeComponent();
        Root.LocationChanged += OnLocationChanged;
        Root.SizeChanged += OnSizeChanged;
    }

    private void OnLocationChanged(object? sender, EventArgs e)
    {
        SynchronizeWindowPosition();
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        SynchronizeWindowPosition();
    }

    public void SynchronizeWindowPosition()
    {
        RequestSynchronizeWindowPosition?.Invoke(Top, Left + ActualWidth);
    }
}
