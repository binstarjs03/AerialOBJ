using System.Windows.Input;
using binstarjs03.MineSharpOBJ.WpfApp.Views;
namespace binstarjs03.MineSharpOBJ.WpfApp.ViewModels;

public class MainViewModel : ViewModelBase<MainViewModel, MainView> {
    private bool _isDebugLogWindowVisible;

    public MainViewModel(MainView view) : base(view) {
        // initialize states
        _isDebugLogWindowVisible = false;

        // assign command implementation to commands
        OpenAboutView = new RelayCommand(OnOpenAboutView);
    }

    // States -----------------------------------------------------------------

    public bool IsDebugLogViewVisible {
        get { return _isDebugLogWindowVisible; }
        set {
            if (value == _isDebugLogWindowVisible)
                return;
            _isDebugLogWindowVisible = value;
            DebugLogViewModel.Context.IsVisible = value;
            OnPropertyChanged(nameof(IsDebugLogViewVisible));
        }
    }

    public bool IsClosing { get; set; }

    // Commands ---------------------------------------------------------------

    public ICommand OpenAboutView { get; }

    // Command Implementations ------------------------------------------------

    private void OnOpenAboutView(object? arg) {
        new AboutView().ShowDialog();
    }
}