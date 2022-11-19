using System.Windows;
using System.Windows.Controls;

using binstarjs03.AerialOBJ.WpfAppNew.Components.Interfaces;

namespace binstarjs03.AerialOBJ.WpfAppNew.View;

public partial class ViewportControl : UserControl
{
    public ViewportControl()
    {
        InitializeComponent();
        (DataContext as IViewport)!.RequestFocusToViewport += OnFocusToViewportRequested;
    }

    private void OnFocusToViewportRequested()
    {
        ViewportPanel.Focus();
    }
}
