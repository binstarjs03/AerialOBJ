using System;
using System.ComponentModel;
using System.Windows;

namespace binstarjs03.AerialOBJ.WpfApp.UIElements.Windows;

public partial class MainWindow : Window
{
    public DebugLogWindow DebugLogWindow { get; }

    public MainWindow(DebugLogWindow debugLogWindow)
    {
        InitializeComponent();
        DataContext = new MainWindowVM(this);
        DebugLogWindow = debugLogWindow;
    }

    private void SynchronizeDebugLogWindowPosition()
    {
        DebugLogWindow.Top = Top;
        DebugLogWindow.Left = Left + ActualWidth;
    }

    private void OnLocationChanged(object sender, EventArgs e)
    {
        SynchronizeDebugLogWindowPosition();
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        SynchronizeDebugLogWindowPosition();
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        // TODO do proper cleanup before exiting
        Environment.Exit(0);
        //Application.Current.Shutdown();
    }
}
