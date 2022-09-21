using System.ComponentModel;
using System.Windows.Controls;

namespace binstarjs03.MineSharpOBJ.WpfApp.UIElements;

public abstract class ViewModelBase<T, U> : INotifyPropertyChanged where T : class where U : Control {
    public event PropertyChangedEventHandler? PropertyChanged;
    protected bool _isVisible = true;
    protected U _control;
    private static T? s_context;

    protected ViewModelBase(U control) {
        _control = control;
    }

    /* late-binding of other VM (property change) event listening.
     * By doing this, we start listening to other VM event
     * when they are instantiated and ready, so we have more control over
     * when we want to listen to events
    */
    public virtual void StartEventListening() { }

    /* context is used for static instance reference
     * that is mutable, can change viewmodel instance context at anytime.
     * we added context so we can access instance context is pointing to
     * through the class
    */
    public static T? Context {
        get {
            return s_context;
        }
        set {
            s_context = value;
        }
    }

    // always call this method whenever make change to state properties
    protected void OnPropertyChanged(string propertyName) {
        if (PropertyChanged != null)
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    // getter for the underlying control
    public U Control => _control;

    // States -----------------------------------------------------------------

    public bool IsVisible {
        get { return _isVisible; }
        set {
            if (value == _isVisible)
                return;
            _isVisible = value;
            OnPropertyChanged(nameof(IsVisible));
        }
    }

    // Event Handlers ---------------------------------------------------------

    protected virtual void OnOtherViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e) { }
}