using System.Windows;

using binstarjs03.CubeOBJ.WpfApp.Services;

namespace binstarjs03.CubeOBJ.WpfApp.UIElements.Windows;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowVM(this);
        
        DebugLogWindow = new DebugLogWindow();
        Show();
        DebugLogWindow.Owner = this;
    }

    public DebugLogWindow DebugLogWindow { get; }

    private void SynchronizeDebugLogWindowPosition()
    {
        DebugLogWindow.Top = Top;
        DebugLogWindow.Left = Left + ActualWidth;
    }

    private void OnLocationChanged(object sender, System.EventArgs e)
    {
        SynchronizeDebugLogWindowPosition();
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        SynchronizeDebugLogWindowPosition();
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        MainService.Initialize();

    }
}
