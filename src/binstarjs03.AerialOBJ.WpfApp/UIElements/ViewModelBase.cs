using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace binstarjs03.AerialOBJ.WpfApp.UIElements;

public abstract class ViewModelBase<TViewModel, TControl> : INotifyPropertyChanged where TViewModel : class where TControl : Control
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected ViewModelBase(TControl control)
    {
        Control = control;
    }

    public TControl Control { get; }

    protected Visibility _visibility;
    public Visibility Visibility
    {
        get => _visibility;
        set => SetAndNotifyPropertyChanged(value, ref _visibility);
    }

    protected void SetPropertyChanged<T>(T newValue, ref T oldValue)
    {
        if (newValue is null || oldValue is null)
            throw new ArgumentNullException
            (
                "newValue or oldValue",
                "Argument passed to ValueChanged of ViewModelBase is null"
            );
        if (newValue.Equals(oldValue))
            return;
        oldValue = newValue;
    }

    // setter also notifier for private fields (non-shared property)
    protected void SetAndNotifyPropertyChanged<T>(T newValue, ref T oldValue, Action? postAction = null, [CallerMemberName] string propertyName = "")
    {
        SetPropertyChanged(newValue, ref oldValue);
        NotifyPropertyChanged(propertyName);
        postAction?.Invoke();
    }

    protected void SetAndNotifyPropertyChanged<T>(T newValue, ref T oldValue, string[] propertyNames, Action? postAction = null, [CallerMemberName] string propertyName = "")
    {
        SetPropertyChanged(newValue, ref oldValue);
        NotifyPropertyChanged(propertyName);
        NotifyPropertyChanged(propertyNames);
        postAction?.Invoke();
    }

    // notifier only
    public void NotifyPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public void NotifyPropertyChanged(string[] propertyNames)
    {
        foreach (string propertyName in propertyNames)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
