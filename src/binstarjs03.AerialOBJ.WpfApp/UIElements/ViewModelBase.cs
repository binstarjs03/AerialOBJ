/*
Copyright (c) 2022, Bintang Jakasurya
All rights reserved. 

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/



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
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
