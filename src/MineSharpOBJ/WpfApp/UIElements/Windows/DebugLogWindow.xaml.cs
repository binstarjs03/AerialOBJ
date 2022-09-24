using System.ComponentModel;
using System.Windows;

namespace binstarjs03.MineSharpOBJ.WpfApp.UIElements.Windows;

public partial class DebugLogWindow : Window
{
    public DebugLogWindow()
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
