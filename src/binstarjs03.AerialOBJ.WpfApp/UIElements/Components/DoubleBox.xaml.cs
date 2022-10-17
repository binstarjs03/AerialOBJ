using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

using binstarjs03.AerialOBJ.WpfApp.Converters;

namespace binstarjs03.AerialOBJ.WpfApp.UIElements.Components;

public partial class DoubleBox : UserControl
{
    public DoubleBox()
    {
        InitializeComponent();
    }

    public double DoubleValue
    {
        get => (double)GetValue(DoubleValueProperty);
        set => SetValue(DoubleValueProperty, value);
    }

    public static readonly DependencyProperty DoubleValueProperty =
        DependencyProperty.Register(nameof(DoubleValue), typeof(double), typeof(DoubleBox), new PropertyMetadata(0.0));

    private void OnKeyUp(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
            UpdateValueBinding();
    }

    private void OnLostFocus(object sender, RoutedEventArgs e)
    {
        UpdateValueBinding();
    }

    private void UpdateValueBinding()
    {
        // bad idea, this is not updating the binding, it bypasses validation rules etc etc, it just make things worse
        //DoubleValue = (double)new DoubleToString().ConvertBack(UnderlyingTextBox.Text, null, null, null);

        // Originally, updating was done using below code,
        // but it fails to update when focus is lost and it still remains mystery.
        BindingExpression expr = UnderlyingTextBox.GetBindingExpression(TextBox.TextProperty);
        expr.UpdateSource();
    }
}
