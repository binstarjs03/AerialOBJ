using System.ComponentModel;
using System.Windows;

namespace binstarjs03.MineSharpOBJ.WpfApp.UIElements.Windows;

public partial class DebugLogWindow : Window, IViewModel<DebugLogWindowViewModel, DebugLogWindow> {
    public DebugLogWindow(MainWindow mainWindow) {
        MainWindow = mainWindow;
        InitializeComponent();
        ViewModel = new DebugLogWindowViewModel(this);
        DebugLogWindowViewModel.Context = ViewModel;
        DataContext = ViewModel;
    }

    public DebugLogWindowViewModel ViewModel { get; private set; }

    public MainWindow MainWindow { get; private set; }

    protected override void OnClosing(CancelEventArgs e) {
        e.Cancel = true;
        Hide();
    }
}
