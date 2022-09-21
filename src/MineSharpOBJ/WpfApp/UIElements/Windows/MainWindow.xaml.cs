using System;
using System.ComponentModel;
using System.Windows;

using binstarjs03.MineSharpOBJ.WpfApp.Services;
using binstarjs03.MineSharpOBJ.WpfApp.UIElements.Controls;

namespace binstarjs03.MineSharpOBJ.WpfApp.UIElements.Windows;

public partial class MainWindow : Window, IViewModel<MainWindowViewModel, MainWindow> {
    public MainWindow() {
        InitializeComponent();
        ViewModel = new MainWindowViewModel(this);
        MainWindowViewModel.Context = ViewModel;
        DataContext = ViewModel;

        DebugLogWindow = new DebugLogWindow(this);
        ViewportControl = new ViewportControl(this);

        Show();
        DebugLogWindow.Owner = this;
        _viewportContainer.Content = ViewportControl;

        SetupViewModelEventListeners();
        MainService.Initialize();
    }

    public MainWindowViewModel ViewModel { get; private set; }
    public DebugLogWindow DebugLogWindow { get; private set; }
    public ViewportControl ViewportControl { get; private set; }

    private void SetupViewModelEventListeners() {
        ViewModel.StartEventListening();
        DebugLogWindow.ViewModel.StartEventListening();
        ViewportControlViewModel.Context!.StartEventListening();
    }

    private void DebugLogView_SynchronizePosition() {
        DebugLogWindow.Top = Top;
        DebugLogWindow.Left = Left + ActualWidth;
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
