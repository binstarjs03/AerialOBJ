using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using binstarjs03.MineSharpOBJ.WpfApp.ViewModels;
namespace binstarjs03.MineSharpOBJ.WpfApp.Views;

public partial class MainView : Window {
    private readonly DebugLogView _debugLogView;
    public MainView() {
        InitializeComponent();
        DataContext = MainViewModel.GetInstance;
        _debugLogView = new DebugLogView();
        Show();
        _debugLogView.Owner = this;
    }

    public void DebugLogView_SynchronizePosition() {
        _debugLogView.Top = Top;
        _debugLogView.Left = Left + ActualWidth;
    }
    protected override void OnLocationChanged(EventArgs e) {
        DebugLogView_SynchronizePosition();
    }

    protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo) {
        DebugLogView_SynchronizePosition();
    }

    protected override void OnClosing(CancelEventArgs e) {
        // TODO do some cleanups here
    }

    protected override void OnClosed(EventArgs e) {
        // TODO do some cleanups here
    }
}