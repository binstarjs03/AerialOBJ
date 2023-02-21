using System.Windows.Controls;

using binstarjs03.AerialOBJ.MvvmAppCore.ViewModels;

using Microsoft.Extensions.DependencyInjection;

namespace binstarjs03.AerialOBJ.WpfApp.Views;
public partial class ViewportInfoControl : UserControl
{
    public ViewportInfoControl()
    {
        InitializeComponent();
        if (App.Current is null)
            return;
        DataContext = App.Current.ServiceProvider.GetRequiredService<ViewportViewModel>();
    }
}
