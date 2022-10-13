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

namespace binstarjs03.MineSharpOBJ.UIPrototype.UIElements.Components;

public partial class NamedControl : UserControl
{
    public string Title
    {
        get { return (string)GetValue(TitleProperty); }
        set { SetValue(TitleProperty, value); }
    }

    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register("Title", typeof(string), typeof(NamedControl), new PropertyMetadata(string.Empty));



    //public object MyProperty
    //{
    //    get { return GetValue(MyPropertyProperty); }
    //    set { SetValue(MyPropertyProperty, value); }
    //}

    //public static readonly DependencyProperty MyPropertyProperty =
    //    DependencyProperty.Register("MyProperty", typeof(object), typeof(ownerclass), new PropertyMetadata(0));



    public NamedControl()
    {
        InitializeComponent();
    }
}
