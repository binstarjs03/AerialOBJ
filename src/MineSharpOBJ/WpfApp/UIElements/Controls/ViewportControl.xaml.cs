using System.Windows.Controls;

namespace binstarjs03.MineSharpOBJ.WpfApp.UIElements.Controls;

public partial class ViewportControl : UserControl
{
    public ViewportControl()
    {
        InitializeComponent();
        ViewportControlVM vm = new(this);
        DataContext = vm;
    }
}
