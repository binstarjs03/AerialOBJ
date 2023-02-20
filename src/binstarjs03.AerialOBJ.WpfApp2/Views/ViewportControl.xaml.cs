using System.Windows.Controls;

using binstarjs03.AerialOBJ.Core;
using binstarjs03.AerialOBJ.Core.Primitives;
using binstarjs03.AerialOBJ.MvvmAppCore.ViewModels;

using Microsoft.Extensions.DependencyInjection;

namespace binstarjs03.AerialOBJ.WpfApp.Views;
public partial class ViewportControl : UserControl
{
    public ViewportControl()
    {
        InitializeComponent();
        if (App.Current is null)
            return;
        var viewmodel = App.Current.ServiceProvider.GetRequiredService<MonolithViewportViewModel>();
        viewmodel.ViewportSizeProvider = GetViewportSize;
        DataContext = viewmodel;
    }

    private Size<int> GetViewportSize()
    {
        return new Size<int>(ActualWidth.Floor(), ActualHeight.Floor());
    }
}
