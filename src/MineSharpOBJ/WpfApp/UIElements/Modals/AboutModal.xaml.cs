using System.Windows;

namespace binstarjs03.MineSharpOBJ.WpfApp.UIElements.Modals;

public partial class AboutModal : Window {
    public AboutModal() {
        InitializeComponent();
        AboutModalViewModel vm = new(this);
        AboutModalViewModel.Context = vm;
        DataContext = vm;
    }
}
