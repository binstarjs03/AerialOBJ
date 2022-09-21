using System.Windows.Controls;

namespace binstarjs03.MineSharpOBJ.WpfApp.UIElements.Controls;

public partial class ViewportDebugControl : UserControl {
    public ViewportDebugControl() {
        InitializeComponent();
        ViewportDebugControlViewModel vm = new(this);
        ViewportDebugControlViewModel.Context = vm;
        DataContext = vm;
    }
}
