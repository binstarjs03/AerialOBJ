using System.Windows;

namespace binstarjs03.MineSharpOBJ.WpfApp.UIElements.Modals;

public partial class AboutModal : Window {
    public AboutModal() {
        InitializeComponent();
    }

    private void OnClose(object sender, RoutedEventArgs e) {
        Close();
    }
}
