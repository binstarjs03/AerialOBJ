using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace binstarjs03.CubeOBJ.WpfApp.UIElements.Components;

public partial class IntBox : TextBox
{
    public IntBox()
    {
        InitializeComponent();
    }

    public int IntValue
    {
        get { return (int)GetValue(IntValueProperty); }
        set { SetValue(IntValueProperty, value); }
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
        BindingExpression expr = Root.GetBindingExpression(TextProperty);
        expr.UpdateSource();
    }
}
