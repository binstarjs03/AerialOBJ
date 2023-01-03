using System.Windows.Controls;

using binstarjs03.AerialOBJ.Core;
using binstarjs03.AerialOBJ.Core.Primitives;
using binstarjs03.AerialOBJ.WpfApp.ViewModels;

namespace binstarjs03.AerialOBJ.WpfApp.Views;
public partial class ViewportView : UserControl, IView
{
    public ViewportView(ViewportViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        viewModel.SetViewportScreenSizeRequested += OnViewModel_ViewportSizeRequested;
    }

    private void OnViewModel_ViewportSizeRequested(ref Size<int> screenSize)
    {
        screenSize = new Size<int>(Viewport.ActualWidth.Floor(),
                                   Viewport.ActualHeight.Floor());
    }
}
