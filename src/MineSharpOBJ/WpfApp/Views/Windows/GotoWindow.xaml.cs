using System.Windows;
using binstarjs03.MineSharpOBJ.WpfApp.ViewModels.Windows;
namespace binstarjs03.MineSharpOBJ.WpfApp.Views.Windows;

public partial class GotoWindow : Window {
    public GotoWindow() {
        InitializeComponent();
        GotoWindowViewModel vm = new(this);
        GotoWindowViewModel.Context = vm;
        DataContext = vm;
    }
}
