using System;
using System.ComponentModel;
using System.Windows;
using binstarjs03.MineSharpOBJ.WpfApp.ViewModels;
namespace binstarjs03.MineSharpOBJ.WpfApp.Views;

public partial class MainView : Window {
    public MainView() {
        InitializeComponent();
        MainViewModel vm = new(this);
        MainViewModel.Context = vm;
        DataContext = vm;

        DebugLogView debugLogView = new();
        Show();
        debugLogView.Owner = this;
    }

    public void DebugLogView_SynchronizePosition() {
        DebugLogViewModel.Context.View.Top = Top;
        DebugLogViewModel.Context.View.Left = Left + ActualWidth;
    }
    protected override void OnLocationChanged(EventArgs e) {
        DebugLogView_SynchronizePosition();
    }

    protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo) {
        DebugLogView_SynchronizePosition();
    }

    protected override void OnClosing(CancelEventArgs e) {
        MainViewModel vm = (MainViewModel)DataContext;
        vm.IsClosing = true;
        // TODO do some cleanups here
    }

    protected override void OnClosed(EventArgs e) {
        // TODO do some cleanups here
        Application.Current.Shutdown(Environment.ExitCode);
    }
}