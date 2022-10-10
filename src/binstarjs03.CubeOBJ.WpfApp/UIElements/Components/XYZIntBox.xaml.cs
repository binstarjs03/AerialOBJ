using System.Windows;
using System.Windows.Controls;

namespace binstarjs03.CubeOBJ.WpfApp.UIElements.Components;

public partial class XYZIntBox : UserControl
{
    public XYZIntBox()
    {
        InitializeComponent();
    }

    public string Header
    {
        get { return (string)GetValue(HeaderProperty); }
        set { SetValue(HeaderProperty, value); }
    }
    public int XValue
    {
        get { return (int)GetValue(XValueProperty); }
        set { SetValue(XValueProperty, value); }
    }
    public int YValue
    {
        get { return (int)GetValue(YValueProperty); }
        set { SetValue(YValueProperty, value); }
    }
    public int ZValue
    {
        get { return (int)GetValue(ZValueProperty); }
        set { SetValue(ZValueProperty, value); }
    }

    public static readonly DependencyProperty HeaderProperty =
        DependencyProperty.Register(nameof(Header), typeof(string), typeof(XYZIntBox), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty XValueProperty =
        DependencyProperty.Register(nameof(XValue), typeof(int), typeof(XYZIntBox), new PropertyMetadata(0));

    public static readonly DependencyProperty YValueProperty =
        DependencyProperty.Register(nameof(YValue), typeof(int), typeof(XYZIntBox), new PropertyMetadata(0));

    public static readonly DependencyProperty ZValueProperty =
        DependencyProperty.Register(nameof(ZValue), typeof(int), typeof(XYZIntBox), new PropertyMetadata(0));
}
