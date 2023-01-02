using System.Windows;

using binstarjs03.AerialOBJ.WpfApp.ViewModels;

namespace binstarjs03.AerialOBJ.WpfApp.Views;
public partial class MainView : Window
{
    public MainView(MainViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        viewModel.CloseViewRequested += OnViewModelCloseRequested;
        Root.LocationChanged += OnLocationChanged;
        Root.SizeChanged += OnSizeChanged;
    }

    public event WindowPositionHandler? DebugViewSetPositionRequested;

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        InvokeDebugViewSetPositionRequested();
    }

    private void OnLocationChanged(object? sender, System.EventArgs e)
    {
        InvokeDebugViewSetPositionRequested();
    }

    public void InvokeDebugViewSetPositionRequested()
    {
        DebugViewSetPositionRequested?.Invoke(Top, Left + ActualWidth);
    }

    private void OnViewModelCloseRequested()
    {
        Close();
    }
}
