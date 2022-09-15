using System.Windows;
using binstarjs03.MineSharpOBJ.WpfApp.ViewModels;
namespace binstarjs03.MineSharpOBJ.WpfApp.Views;

public partial class AboutView : Window {
    public AboutView() {
        InitializeComponent();
        DataContext = new AboutViewModel(this);
    }
}
