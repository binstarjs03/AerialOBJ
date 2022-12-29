using System;
using System.Windows.Controls;

using binstarjs03.AerialOBJ.Core;
using binstarjs03.AerialOBJ.Core.Primitives;
using binstarjs03.AerialOBJ.WpfAppNew2.ViewModels;

using Microsoft.Extensions.DependencyInjection;

namespace binstarjs03.AerialOBJ.WpfAppNew2.Views;
public partial class ViewportView : UserControl
{
    public ViewportView()
    {
        InitializeComponent();
        DataContext = App.Current?.Host.Services.GetRequiredService<ViewportViewModel>();
        (DataContext as ViewportViewModel)!.SetViewportSizeRequested += OnViewModel_ViewportSizeRequested;
    }

    private void OnViewModel_ViewportSizeRequested()
    {
        (DataContext as ViewportViewModel)!.ScreenSize = new Size<int>(Viewport.ActualWidth.Floor(),
                                                                       Viewport.ActualHeight.Floor());
    }
}
