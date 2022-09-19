using System.Windows.Controls;

using binstarjs03.MineSharpOBJ.WpfApp.ViewModels.Controls;
namespace binstarjs03.MineSharpOBJ.WpfApp.Views.Controls;

public partial class ViewportDebugControl : UserControl {
    public ViewportDebugControl() {
        InitializeComponent();
        ViewportDebugControlViewModel vm = new(this);
        ViewportDebugControlViewModel.Context = vm;
        DataContext = vm;
    }
}
