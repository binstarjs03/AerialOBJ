using System.Windows;
using binstarjs03.MineSharpOBJ.WpfApp.ViewModels.Windows;
namespace binstarjs03.MineSharpOBJ.WpfApp.Views.Windows;

public partial class AboutWindow : Window {
    public AboutWindow() {
        InitializeComponent();
        AboutWindowViewModel vm = new(this);
        AboutWindowViewModel.Context = vm;
        DataContext = vm;
    }
}
