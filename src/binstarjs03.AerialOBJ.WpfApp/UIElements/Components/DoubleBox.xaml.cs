using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace binstarjs03.AerialOBJ.WpfApp.UIElements.Components;

public partial class DoubleBox : UserControl, INotifyPropertyChanged
{
    public DoubleBox()
    {
        InitializeComponent();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public double DoubleValue
    {
        get => (double)GetValue(DoubleValueProperty);
        set => SetValue(DoubleValueProperty, value);
    }

    public static readonly DependencyProperty DoubleValueProperty =
        DependencyProperty.Register(nameof(DoubleValue), typeof(double), typeof(DoubleBox), new PropertyMetadata(0.0));

    // we add our own focused property, this is to let the binding to update
    // using the non-formatted value when focus is lost (see OnLostFocus),
    // then after binding is updated, we set focus to false to display the formatted value
    private bool _focused = false;
    public bool Focused
    {
        get => _focused;
        set
        {
            _focused = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Focused)));
        }
    }

    private void OnKeyUp(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
            UpdateValueBinding();
    }

    private void OnLostFocus(object sender, RoutedEventArgs e)
    {
        UpdateValueBinding();
        Focused = false;
    }

    private void OnGotFocus(object sender, RoutedEventArgs e)
    {
        Focused = true;
    }

    private void UpdateValueBinding()
    {
        BindingExpression expr = UnderlyingTextBox.GetBindingExpression(TextBox.TextProperty);
        expr.UpdateSource();
    }
}
