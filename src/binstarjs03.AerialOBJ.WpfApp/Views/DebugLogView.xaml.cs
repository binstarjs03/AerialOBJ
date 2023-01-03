using System.ComponentModel;
using System.Windows;

using binstarjs03.AerialOBJ.WpfApp.ViewModels;

namespace binstarjs03.AerialOBJ.WpfApp.Views;
public partial class DebugLogView : Window, IClosableView
{
    public DebugLogView(DebugLogViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        viewModel.ScrollToEndRequested += OnViewModelScrollToEndRequested;
    }

    public void SetTopLeft(double top, double left)
    {
        Top = top;
        Left = left;
    }

    private void OnViewModelScrollToEndRequested()
    {
        LogTextBox.ScrollToEnd();
    }

    // Debug log window do not close. They just hide.
    protected override void OnClosing(CancelEventArgs e)
    {
        e.Cancel = true;
        Hide();
    }
}
