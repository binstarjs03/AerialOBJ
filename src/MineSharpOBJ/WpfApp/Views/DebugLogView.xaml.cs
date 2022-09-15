using System.ComponentModel;
using System.Windows;
using binstarjs03.MineSharpOBJ.WpfApp.ViewModels;
namespace binstarjs03.MineSharpOBJ.WpfApp.Views;

public partial class DebugLogView : Window {
    public DebugLogView() {
        InitializeComponent();
        DebugLogViewModel vm = new(this);
        DebugLogViewModel.Context = vm;
        DataContext = vm;
    }

    protected override void OnClosing(CancelEventArgs e) {
        e.Cancel = true;
        Hide();
    }
}
