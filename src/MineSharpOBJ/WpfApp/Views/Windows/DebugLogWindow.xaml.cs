using System.ComponentModel;
using System.Windows;
using binstarjs03.MineSharpOBJ.WpfApp.ViewModels.Windows;
namespace binstarjs03.MineSharpOBJ.WpfApp.Views.Windows;

public partial class DebugLogWindow : Window {
    public DebugLogWindow() {
        InitializeComponent();
        DebugLogWindowViewModel vm = new(this);
        DebugLogWindowViewModel.Context = vm;
        DataContext = vm;
    }

    protected override void OnClosing(CancelEventArgs e) {
        e.Cancel = true;
        Hide();
    }
}
