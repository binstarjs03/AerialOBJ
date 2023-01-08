using System.Windows;

using binstarjs03.AerialOBJ.WpfApp.ViewModels;

namespace binstarjs03.AerialOBJ.WpfApp.Views;
public partial class MainView : Window, IClosableView
{
    public MainView(MainViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        Root.LocationChanged += OnLocationChanged;
        Root.SizeChanged += OnSizeChanged;
    }

    public event WindowPositionHandler? DebugViewSyncPositionRequested;
    private void OnSizeChanged(object sender, SizeChangedEventArgs e) => RequestDebugViewsSyncPosition();
    private void OnLocationChanged(object? sender, System.EventArgs e) => RequestDebugViewsSyncPosition();
    public void RequestDebugViewsSyncPosition() => DebugViewSyncPositionRequested?.Invoke(Top, Left + ActualWidth);
}
