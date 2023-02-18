using System.ComponentModel;
using System.Windows;

using binstarjs03.AerialOBJ.MVVM.Services.ViewServices;
using binstarjs03.AerialOBJ.MVVM.ViewModels;

using Microsoft.Extensions.DependencyInjection;

namespace binstarjs03.AerialOBJ.WpfApp.Views;
public partial class DebugLogWindow : Window, IClosable, IScrollable
{
    public DebugLogWindow()
    {
        InitializeComponent();
        var viewmodel = App.Current.ServiceProvider.GetRequiredService<DebugLogViewModel>();
        viewmodel.Closable = this;
        viewmodel.Scrollable = this;
        DataContext = viewmodel;
    }

    public void ScrollToEnd() => LogTextBox.ScrollToEnd();

    protected override void OnClosing(CancelEventArgs e)
    {
        e.Cancel= true;
        Hide();
    }
}
