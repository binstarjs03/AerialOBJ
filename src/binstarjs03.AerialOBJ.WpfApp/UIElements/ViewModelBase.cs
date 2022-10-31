using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace binstarjs03.AerialOBJ.WpfApp.UIElements;

public abstract class ViewModelBase<T, U> : INotifyPropertyChanged where T : class where U : Control
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected ViewModelBase(U control)
    {
        Control = control;
    }

    public U Control { get; }

    protected Visibility _visibility;
    public Visibility Visibility
    {
        get => _visibility;
        set => SetAndNotifyPropertyChanged(value, ref _visibility);
    }

    // setter also notifier for private fields (non-shared property)
    protected void SetAndNotifyPropertyChanged<V>(V newValue, ref V oldValue, [CallerMemberName] string propertyName = "")
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
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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

    protected virtual void OnSharedPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        string propName = e.PropertyName!;
        NotifyPropertyChanged(propName);
    }
}
