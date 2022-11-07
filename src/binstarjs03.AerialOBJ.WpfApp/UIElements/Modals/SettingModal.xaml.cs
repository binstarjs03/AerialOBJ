using System.Windows;

namespace binstarjs03.AerialOBJ.WpfApp.UIElements.Modals;
/// <summary>
/// Interaction logic for SettingModal.xaml
/// </summary>
public partial class SettingModal : Window
{
    public SettingModal()
    {
        InitializeComponent();
    }

    private void OnClickOK(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
