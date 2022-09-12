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

public partial class DebugLogView : Window {
    public DebugLogView() {
        InitializeComponent();
        DataContext = DebugLogViewModel.GetInstance;
        DebugLogViewModel.GetInstance.DebugLogView = this;
    }

    protected override void OnClosing(CancelEventArgs e) {
        e.Cancel = true;
        Hide();
    }
}
