using System.ComponentModel;
using System.Windows;

using binstarjs03.AerialOBJ.WpfAppNew.Components.Interfaces;

namespace binstarjs03.AerialOBJ.WpfAppNew.View;

public partial class DebugLogWindow : Window, IClosable
{
    public DebugLogWindow()
    {
        InitializeComponent();
        (DataContext as IScroller)!.RequestScrollToEnd += OnScrollToEndRequested;
    }

    public void OnScrollToEndRequested()
    {
        LogTextBox.ScrollToEnd();
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        e.Cancel = true;
        Hide();
    }

    public void OnSynchronizeWindowPositionRequested(double top, double left)
    {
        Top = top;
        Left = left;
    }
}
