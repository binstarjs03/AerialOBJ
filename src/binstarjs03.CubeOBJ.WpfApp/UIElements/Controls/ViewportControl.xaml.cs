using System.Windows.Controls;

namespace binstarjs03.CubeOBJ.WpfApp.UIElements.Controls;

public partial class ViewportControl : UserControl
{
    public ViewportControl()
    {
        InitializeComponent();
        DataContext = new ViewportControlVM(this);
    }
}
