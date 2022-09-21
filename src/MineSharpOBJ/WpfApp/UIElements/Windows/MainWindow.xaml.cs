using System;
using System.ComponentModel;
using System.Windows;

using binstarjs03.MineSharpOBJ.WpfApp.Services;
using binstarjs03.MineSharpOBJ.WpfApp.UIElements.Controls;

namespace binstarjs03.MineSharpOBJ.WpfApp.UIElements.Windows;

public partial class MainWindow : Window {
    private readonly MainWindowViewModel _vm;

    public MainWindow() {
        _vm = new(this);
        MainWindowViewModel.Context = _vm;
        DataContext = _vm;
        InitializeComponent();
        InstantiateStandbySecondWindows();
        SetupViewModelEventListeners();
        MainService.Initialize();
    }

    private void InstantiateStandbySecondWindows() {
        DebugLogWindow debugLogView = new();
        Show();
        debugLogView.Owner = this;
    }

    private void SetupViewModelEventListeners() {
        _vm.StartEventListening();
        DebugLogWindowViewModel.Context!.StartEventListening();
        ViewportControlViewModel.Context!.StartEventListening();
    }

    private void DebugLogView_SynchronizePosition() {
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
        // TODO do something here (but dont cleanup yet)
    }

    protected override void OnClosed(EventArgs e) {
        // TODO do some cleanups here
        Application.Current.Shutdown(Environment.ExitCode);
    }
}
