using System.Windows;

using binstarjs03.AerialOBJ.WpfApp.ViewModels;

namespace binstarjs03.AerialOBJ.WpfApp.Views;
public partial class MainView : Window, IClosableView
{
    public MainView(IMainViewModel mainViewModel)
    {
        InitializeComponent();
        DataContext = mainViewModel;
        Root.LocationChanged += OnLocationChanged;
        Root.SizeChanged += OnSizeChanged;
    }

    public event WindowPositionHandler? RequestSetDebugViewPosition;
    private void OnSizeChanged(object sender, SizeChangedEventArgs e) => SyncDebugViewPosition();
    private void OnLocationChanged(object? sender, System.EventArgs e) => SyncDebugViewPosition();
    public void SyncDebugViewPosition() => RequestSetDebugViewPosition?.Invoke(Top, Left + ActualWidth);
}
