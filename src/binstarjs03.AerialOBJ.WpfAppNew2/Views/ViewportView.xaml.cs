using System;
using System.Windows.Controls;

using binstarjs03.AerialOBJ.Core;
using binstarjs03.AerialOBJ.Core.Primitives;
using binstarjs03.AerialOBJ.WpfApp.ViewModels;

using Microsoft.Extensions.DependencyInjection;

namespace binstarjs03.AerialOBJ.WpfApp.Views;
public partial class ViewportView : UserControl
{
    public ViewportView()
    {
        InitializeComponent();
        DataContext = App.Current?.Host.Services.GetRequiredService<ViewportViewModel>();
        (DataContext as ViewportViewModel)!.SetViewportScreenSizeRequested += OnViewModel_ViewportSizeRequested;
    }

    private void OnViewModel_ViewportSizeRequested(ref Size<int> screenSize)
    {
        screenSize = new Size<int>(Viewport.ActualWidth.Floor(),
                                   Viewport.ActualHeight.Floor());
    }
}
