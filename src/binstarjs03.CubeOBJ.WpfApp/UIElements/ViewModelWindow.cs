using System.Windows;
using System.Windows.Input;

namespace binstarjs03.CubeOBJ.WpfApp.UIElements;

public abstract class ViewModelWindow<T, U> : ViewModelBase<T, U> where T : class where U : Window
{
    protected ViewModelWindow(U window) : base(window)
    {

        // assign command implementation to commands
        WindowCloseCommand = new RelayCommand(OnWindowClose);
        Window = window;
    }

    // Accessors --------------------------------------------------------------

    public U Window { get; }

    // Commands ---------------------------------------------------------------

    public ICommand WindowCloseCommand { get; }
    protected virtual void OnWindowClose(object? arg)
    {
        Window.Close();
    }
}
