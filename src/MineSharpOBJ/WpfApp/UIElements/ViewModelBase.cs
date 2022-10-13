using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace binstarjs03.MineSharpOBJ.WpfApp.UIElements;

public abstract class ViewModelBase<T, U> : INotifyPropertyChanged where T : class where U : Control
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected ViewModelBase(U control)
    {
        Control = control;
    }

    // Accessors --------------------------------------------------------------

    public U Control { get; }

    // States -----------------------------------------------------------------

    protected Visibility _visibility;
    public Visibility Visibility
    {
        get => _visibility;
        set => SetAndNotifyPropertyChanged(value, ref _visibility);
    }

    // Methods ----------------------------------------------------------------

    // setter also notifier for private fields (non-shared property)
    protected void SetAndNotifyPropertyChanged<V>(V newValue, ref V oldValue, [CallerMemberName] string propertyName = "")
    {
        if (newValue is null || oldValue is null)
            throw new ArgumentNullException
            (
                "newValue or oldValue",
                "Argument passed to SetAndNotifyPropertyChanged of ViewModelBase is null"
            );
        if (newValue.Equals(oldValue))
            return;
        oldValue = newValue;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    // notifier only
    protected void NotifyPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }


    // setter for shared property. Note that we cannot ref property so
    // we set the property value through delegate
    protected void SetSharedPropertyChanged<V>(V newValue, Action<V> setterMethod)
    {
        setterMethod(newValue);
    }
    // Event Handlers ---------------------------------------------------------

    protected virtual void OnSharedPropertyChanged(object? sender, PropertyChangedEventArgs e) {
        NotifyPropertyChanged(e.PropertyName!);
    }
}