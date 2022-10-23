using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace binstarjs03.AerialOBJ.WpfApp.UIElements.Components;

public partial class IntBox : UserControl, INumericBox
{
    public IntBox()
    {
        InitializeComponent();
    }

    public int IntValue
    {
        get => (int)GetValue(IntValueProperty);
        set => SetValue(IntValueProperty, value);
    }

    public static readonly DependencyProperty IntValueProperty =
        DependencyProperty.Register(nameof(IntValue), typeof(int), typeof(IntBox), new PropertyMetadata(0));

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

    public void ClearValue()
    {
        IntValue = 0;
    }
}
