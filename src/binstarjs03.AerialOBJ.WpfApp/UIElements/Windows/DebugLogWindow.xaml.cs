using System.ComponentModel;
using System.Windows;

namespace binstarjs03.AerialOBJ.WpfApp.UIElements.Windows;

public partial class DebugLogWindow : Window
{
    public DebugLogWindow()
    {
        InitializeComponent();
        DataContext = new DebugLogWindowVM(this);
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        e.Cancel = true;
        Hide();
    }
}
