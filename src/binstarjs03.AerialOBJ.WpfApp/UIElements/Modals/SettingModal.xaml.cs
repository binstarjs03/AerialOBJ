using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
