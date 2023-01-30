using System.Windows.Controls;
using System.Windows.Input;

using binstarjs03.AerialOBJ.Core;
using binstarjs03.AerialOBJ.Core.Primitives;
using binstarjs03.AerialOBJ.WpfApp.ViewModels;

using Microsoft.Extensions.DependencyInjection;

namespace binstarjs03.AerialOBJ.WpfApp.Views;
public partial class ViewportView : UserControl, IView
{
    public ViewportView()
    {
        InitializeComponent();

        // Dumb designer fix, won't be null at runtime
        if (App.Current is null) 
            return;

        ViewportViewModel viewModel = App.Current.ServiceProvider.GetRequiredService<ViewportViewModel>();
        DataContext = viewModel;
        viewModel.GetViewViewportSize = GetViewportSize;
        Keyboard.Focus(Viewport);
        Viewport.Focus();
    }

    private Size<int> GetViewportSize()
    {
        return new Size<int>(Viewport.ActualWidth.Floor(),
                             Viewport.ActualHeight.Floor());
    }

    private void UserControl_MouseDown(object sender, MouseButtonEventArgs e)
    {
        Keyboard.Focus(Viewport);
        Viewport.Focus();
    }
}
