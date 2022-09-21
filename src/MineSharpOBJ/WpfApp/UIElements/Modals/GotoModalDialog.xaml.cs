using System.Windows;

namespace binstarjs03.MineSharpOBJ.WpfApp.UIElements.Modals;

public partial class GotoModalDialog : Window {
    public GotoModalDialog() {
        InitializeComponent();
        GotoModalDialogViewModel vm = new(this);
        GotoModalDialogViewModel.Context = vm;
        DataContext = vm;
    }
}
