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

namespace binstarjs03.MineSharpOBJ.UIPrototype.UIElements.Windows;
/// <summary>
/// Interaction logic for DebugLogWindow.xaml
/// </summary>
public partial class DebugLogWindow : Window
{
    public DebugLogWindow()
    {
        InitializeComponent();
    }

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        e.Cancel = true;
        Hide();
        ((MainWindow)Owner).DebugLog.IsChecked = false;
    }

    private void MenuItem_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
