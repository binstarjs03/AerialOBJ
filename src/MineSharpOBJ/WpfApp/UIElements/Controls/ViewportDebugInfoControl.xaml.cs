using System.Windows.Controls;

namespace binstarjs03.MineSharpOBJ.WpfApp.UIElements.Controls;

public partial class ViewportDebugInfoControl : UserControl
{
    public ViewportDebugInfoControl()
    {
        InitializeComponent();
        ViewportDebugInfoControlVM vm = new(this);
        DataContext = vm;
    }
}
