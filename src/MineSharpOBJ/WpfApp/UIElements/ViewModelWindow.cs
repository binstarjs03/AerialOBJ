using System.Windows;
using System.Windows.Input;

namespace binstarjs03.MineSharpOBJ.WpfApp.UIElements;

public abstract class ViewModelWindow<T, U> : ViewModelBase<T, U> where T : class where U : Window {
    protected readonly U _window;

    protected ViewModelWindow(U window) : base(window) {
        _window = window;

        // assign command implementation to commands
        WindowClose = new RelayCommand(OnWindowClose);
    }

    // getter for the underlying window
    public U Window => _window;

    // Commands ---------------------------------------------------------------

    public ICommand WindowClose { get; }

    // Command Implementations ------------------------------------------------

    protected virtual void OnWindowClose(object? arg) {
        _window.Close();
    }
}
