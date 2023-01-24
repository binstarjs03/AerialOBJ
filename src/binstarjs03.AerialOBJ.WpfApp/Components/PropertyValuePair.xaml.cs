using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace binstarjs03.AerialOBJ.WpfApp.Components;
public partial class PropertyValuePair : UserControl
{
    public PropertyValuePair()
    {
        InitializeComponent();
    }

    public SolidColorBrush PropertyColor
    {
        get { return (SolidColorBrush)GetValue(PropertyColorProperty); }
        set { SetValue(PropertyColorProperty, value); }
    }

    public static readonly DependencyProperty PropertyColorProperty =
        DependencyProperty.Register(nameof(PropertyColor),
                                    typeof(SolidColorBrush),
                                    typeof(PropertyValuePair),
                                    new PropertyMetadata(new SolidColorBrush(Colors.White)));

    public SolidColorBrush ValueColor
    {
        get { return (SolidColorBrush)GetValue(ValueColorProperty); }
        set { SetValue(ValueColorProperty, value); }
    }

    public static readonly DependencyProperty ValueColorProperty =
        DependencyProperty.Register(nameof(ValueColor),
                                    typeof(SolidColorBrush),
                                    typeof(PropertyValuePair),
                                    new PropertyMetadata(new SolidColorBrush(Colors.White)));

    public string PropertyText
    {
        get { return (string)GetValue(PropertyTextProperty); }
        set { SetValue(PropertyTextProperty, value); }
    }

    public static readonly DependencyProperty PropertyTextProperty =
        DependencyProperty.Register(nameof(PropertyText),
                                    typeof(string),
                                    typeof(PropertyValuePair),
                                    new PropertyMetadata(""));

    public string ValueText
    {
        get { return (string)GetValue(ValueTextProperty); }
        set { SetValue(ValueTextProperty, value); }
    }

    public static readonly DependencyProperty ValueTextProperty =
        DependencyProperty.Register(nameof(ValueText),
                                    typeof(string),
                                    typeof(PropertyValuePair),
                                    new PropertyMetadata(""));
}
