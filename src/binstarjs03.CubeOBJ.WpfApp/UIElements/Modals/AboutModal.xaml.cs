using System.Windows;

namespace binstarjs03.CubeOBJ.WpfApp.UIElements.Modals;

public partial class AboutModal : Window
{
    public AboutModal()
    {
        InitializeComponent();
    }

    private void OnClickClose(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
