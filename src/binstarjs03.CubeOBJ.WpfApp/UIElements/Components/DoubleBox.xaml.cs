using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using binstarjs03.CubeOBJ.WpfApp.Converters;

namespace binstarjs03.CubeOBJ.WpfApp.UIElements.Components;

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
        # pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        // i have no idea how to pass to another ConvertBack parameter, so i leave it to null
        DoubleValue = (double)new DoubleToString().ConvertBack(UnderlyingTextBox.Text, null, null, null);
        # pragma warning restore CS8625
        // Originally, updating was done using below code,
        // but it fails to update when focus is lost and it still remains mystery.
        //BindingExpression expr = UnderlyingTextBox.GetBindingExpression(TextBox.TextProperty);
        //expr.UpdateSource();
    }
}
