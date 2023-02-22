using System.Windows.Controls;

using binstarjs03.AerialOBJ.MvvmAppCore.ViewModels;

using Microsoft.Extensions.DependencyInjection;

namespace binstarjs03.AerialOBJ.WpfApp.Views;
public partial class ViewportRegionImagePresenter : UserControl
{
    public ViewportRegionImagePresenter()
    {
        InitializeComponent();
        if (App.Current is null)
            return;
        DataContext = App.Current.ServiceProvider.GetRequiredService<ViewportViewModel>();
    }
}