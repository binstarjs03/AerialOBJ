using System.Windows;
using binstarjs03.MineSharpOBJ.WpfApp.ViewModels.Windows;
namespace binstarjs03.MineSharpOBJ.WpfApp.Views.Windows;

public partial class AboutWindow : Window {
    public AboutWindow() {
        InitializeComponent();
        DataContext = new AboutWindowViewModel(this);
    }
}
