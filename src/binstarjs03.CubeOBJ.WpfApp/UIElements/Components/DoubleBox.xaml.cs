using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

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
        BindingExpression expr = UnderlyingTextBox.GetBindingExpression(TextBox.TextProperty);
        expr.UpdateSource();
    }
}
