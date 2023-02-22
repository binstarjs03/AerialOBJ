using System.Windows;
using System.Windows.Controls;

namespace binstarjs03.AerialOBJ.WpfApp.Views;
public partial class ViewportGridDrawer : UserControl
{
    public ViewportGridDrawer()
    {
        InitializeComponent();
    }



    public bool IsRegionGridVisible
    {
        get => (bool)GetValue(IsRegionGridVisibleProperty);
        set => SetValue(IsRegionGridVisibleProperty, value);
    }

    public static readonly DependencyProperty IsRegionGridVisibleProperty =
        DependencyProperty.Register(nameof(IsRegionGridVisible),
                                    typeof(bool),
                                    typeof(ViewportGridDrawer),
                                    new PropertyMetadata(false));

    public bool IsChunkGridVisible
    {
        get => (bool)GetValue(IsChunkGridVisibleProperty);
        set => SetValue(IsChunkGridVisibleProperty, value);
    }

    public static readonly DependencyProperty IsChunkGridVisibleProperty =
        DependencyProperty.Register(nameof(IsChunkGridVisible),
                                    typeof(bool),
                                    typeof(ViewportGridDrawer),
                                    new PropertyMetadata(false));
}