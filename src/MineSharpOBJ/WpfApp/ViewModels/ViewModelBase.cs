using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;

namespace binstarjs03.MineSharpOBJ.WpfApp.ViewModels;
public abstract class ViewModelBase<T> : INotifyPropertyChanged where T : class, new() {
    public event PropertyChangedEventHandler? PropertyChanged;
    private static T? s_instance;

    protected ViewModelBase() {
        // assign command implementation to commands
        ViewClose = new RelayCommand(OnViewClose);
    }

    // always dereference this property whenever need access to viewmodel
    // using singleton ensures there is only single instance of viewmodel
    public static T GetInstance {
        get {
            s_instance ??= new T();
            return s_instance;
        }
    }

    // always call this method whenever make change to state properties
    protected void OnPropertyChanged(string propertyName) {
        if (PropertyChanged != null)
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    // Commands -----------------------------------------------------------

    public ICommand ViewClose { get; }

    // Command Implementations --------------------------------------------

    protected static void OnViewClose(object? arg) {
        if (arg == null)
            throw new ArgumentNullException(nameof(arg));
        Window window = (Window)arg;
        window.Close();
    }
}