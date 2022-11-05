using System.Windows;
using System.Windows.Input;

namespace binstarjs03.AerialOBJ.WpfApp.UIElements;

public abstract class ViewModelWindow<TViewModel, TWindow> : ViewModelBase<TViewModel, TWindow> where TViewModel : class where TWindow : Window
{
    protected ViewModelWindow(TWindow window) : base(window)
    {
        WindowCloseCommand = new RelayCommand(OnWindowClose);
        Window = window;
    }

    public TWindow Window { get; }

    public ICommand WindowCloseCommand { get; }
    protected virtual void OnWindowClose(object? arg)
    {
        Window.Close();
    }
}
