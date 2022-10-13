using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

using binstarjs03.MineSharpOBJ.UIPrototype.UIElements.Modals;

namespace binstarjs03.MineSharpOBJ.UIPrototype.UIElements.Windows;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly DebugLogWindow _debugLogWindow;

    public MainWindow()
    {
        InitializeComponent();
        DataContext = this;

        _debugLogWindow = new DebugLogWindow();
        Show();
        _debugLogWindow.Owner = this;
    }

    private void DebugLogWindow_SynchronizePosition()
    {
        _debugLogWindow.Top = Top;
        _debugLogWindow.Left = Left + ActualWidth;
    }

    protected override void OnLocationChanged(EventArgs e)
    {
        DebugLogWindow_SynchronizePosition();
    }

    protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
    {
        DebugLogWindow_SynchronizePosition();
    }

    private void DebugLog_Checked(object sender, RoutedEventArgs e)
    {
        _debugLogWindow.Show();
    }

    private void DebugLog_Unchecked(object sender, RoutedEventArgs e)
    {
        _debugLogWindow.Hide();
    }

    private void Exit_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void About_Click(object sender, RoutedEventArgs e)
    {
        new AboutModal().ShowDialog();
    }
}
