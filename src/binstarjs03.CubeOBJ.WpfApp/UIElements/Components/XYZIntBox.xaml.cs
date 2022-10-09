using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
