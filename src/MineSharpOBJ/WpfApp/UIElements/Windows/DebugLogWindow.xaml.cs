using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

using binstarjs03.MineSharpOBJ.WpfApp.BindingConverters;

namespace binstarjs03.MineSharpOBJ.WpfApp.UIElements.Windows;

public partial class DebugLogWindow : Window
{
    public DebugLogWindow(MainWindow owner)
    {
        InitializeComponent();
        DebugLogWindowVM vm = new(this);
        DataContext = vm;
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        e.Cancel = true;
        Visibility = Visibility.Collapsed;
    }
}
