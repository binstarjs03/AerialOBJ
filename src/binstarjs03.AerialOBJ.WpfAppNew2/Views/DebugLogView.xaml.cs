using System.ComponentModel;
using System.Windows;

using binstarjs03.AerialOBJ.WpfAppNew2.ViewModels;

namespace binstarjs03.AerialOBJ.WpfAppNew2.Views;
public partial class DebugLogView : Window
{
    public DebugLogView(DebugLogViewModel viewModel)
    {
        InitializeComponent();
        DataContext= viewModel;
        viewModel.CloseViewRequested += OnViewModelCloseRequested;
    }

    public void SetTopLeft(double top, double left)
    {
        Top = top;
        Left = left;
    }

    private void OnViewModelCloseRequested()
    {
        Close();
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        e.Cancel = true;
        Hide();
    }
}
