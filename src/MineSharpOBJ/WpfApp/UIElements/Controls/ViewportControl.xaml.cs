using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace binstarjs03.MineSharpOBJ.WpfApp.UIElements.Controls;

public partial class ViewportControl : UserControl
{
    public ViewportControl()
    {
        InitializeComponent();
        ViewportControlVM vm = new(this);
        _vm = vm;
        DataContext = vm;
    }

    private ViewportControlVM _vm { get; }

    private void ViewportCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        _vm.OnSizeChanged(sender, e);
    }

    private void ViewportCanvas_MouseWheel(object sender, MouseWheelEventArgs e)
    {
        _vm.OnMouseWheel(sender, e);

    }

    private void ViewportCanvas_MouseMove(object sender, MouseEventArgs e)
    {
        _vm.OnMouseMove(sender, e);

    }

    private void ViewportCanvas_MouseUp(object sender, MouseButtonEventArgs e)
    {
        _vm.OnMouseUp(sender, e);

    }

    private void ViewportCanvas_MouseDown(object sender, MouseButtonEventArgs e)
    {
        _vm.OnMouseDown(sender, e);

    }

    private void ViewportCanvas_MouseLeave(object sender, MouseEventArgs e)
    {
        _vm.OnMouseLeave(sender, e);

    }

    private void ViewportCanvas_MouseEnter(object sender, MouseEventArgs e)
    {
        _vm.OnMouseEnter(sender, e);

    }
}
