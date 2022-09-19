using System;
using System.ComponentModel;
using System.Windows;

using binstarjs03.MineSharpOBJ.WpfApp.Services;
using binstarjs03.MineSharpOBJ.WpfApp.ViewModels.Controls;
using binstarjs03.MineSharpOBJ.WpfApp.ViewModels.Windows;
namespace binstarjs03.MineSharpOBJ.WpfApp.Views.Windows;

public partial class MainWindow : Window {
    public MainWindow() {
        InitializeComponent();
        MainWindowViewModel vm = new(this);
        MainWindowViewModel.Context = vm;
        DataContext = vm;

        DebugLogWindow debugLogView = new();
        Show();
        debugLogView.Owner = this;

        vm.StartEventListening();
        DebugLogWindowViewModel.Context!.StartEventListening();
        ViewportControlViewModel.Context!.StartEventListening();

        MainService.Initialize();
    }

    public void DebugLogView_SynchronizePosition() {
        DebugLogWindowViewModel.Context!.Window.Top = Top;
        DebugLogWindowViewModel.Context!.Window.Left = Left + ActualWidth;
    }
    protected override void OnLocationChanged(EventArgs e) {
        DebugLogView_SynchronizePosition();
    }

    protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo) {
        DebugLogView_SynchronizePosition();
    }

    protected override void OnClosing(CancelEventArgs e) {
        MainWindowViewModel vm = (MainWindowViewModel)DataContext;
        vm.IsClosing = true;
        // TODO do some cleanups here
    }

    protected override void OnClosed(EventArgs e) {
        // TODO do some cleanups here
        Application.Current.Shutdown(Environment.ExitCode);
    }
}