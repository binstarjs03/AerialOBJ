using System;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows;
namespace binstarjs03.MineSharpOBJ.WpfApp.ViewModels;

public abstract class ViewModelBase<T, U> : INotifyPropertyChanged where T : class where U : Window {
    public event PropertyChangedEventHandler? PropertyChanged;
    private static T? s_context;
    protected readonly U _view;

    protected ViewModelBase(U view) {
        _view = view;

        // assign command implementation to commands
        ViewClose = new RelayCommand(OnViewClose);
    }

    // context is used for static instance reference
    // that is mutable, can change viewmodel instance context at anytime.
    // we added context so we can access instance context is pointing to
    // through the class
    public static T Context {
        get {
            if (s_context is null)
                throw new NullReferenceException();
            return s_context;
        }
        set {
            s_context = value;
        }
    }

    // getter for the underlying view
    public U View => _view;

    // always call this method whenever make change to state properties
    protected void OnPropertyChanged(string propertyName) {
        if (PropertyChanged != null)
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    // Commands -----------------------------------------------------------

    public ICommand ViewClose { get; }

    // Command Implementations --------------------------------------------

    protected virtual void OnViewClose(object? arg) {
        _view.Close();
    }
}