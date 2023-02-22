using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

using binstarjs03.AerialOBJ.Core.Primitives;
using binstarjs03.AerialOBJ.MvvmAppCore.Models;

namespace binstarjs03.AerialOBJ.WpfApp.Views;
public partial class ViewportRegionImagePresenter : UserControl
{
    public ViewportRegionImagePresenter()
    {
        InitializeComponent();
    }
    public ObservableCollection<RegionDataImageModel> RegionDataImageModelSource
    {
        get => (ObservableCollection<RegionDataImageModel>)GetValue(RegionDataImageModelSourceProperty);
        set => SetValue(RegionDataImageModelSourceProperty, value);
    }

    public static readonly DependencyProperty RegionDataImageModelSourceProperty =
        DependencyProperty.Register(nameof(RegionDataImageModelSource),
                                    typeof(ObservableCollection<RegionDataImageModel>),
                                    typeof(ViewportRegionImagePresenter),
                                    new PropertyMetadata(null));

    public PointZ<float> CameraPos
    {
        get => (PointZ<float>)GetValue(CameraPosProperty);
        set => SetValue(CameraPosProperty, value);
    }

    public static readonly DependencyProperty CameraPosProperty =
        DependencyProperty.Register(nameof(CameraPos),
                                    typeof(PointZ<float>),
                                    typeof(ViewportRegionImagePresenter),
                                    new PropertyMetadata(PointZ<float>.Zero));

    public Size<int> ScreenSize
    {
        get => (Size<int>)GetValue(ScreenSizeProperty);
        set => SetValue(ScreenSizeProperty, value);
    }

    public static readonly DependencyProperty ScreenSizeProperty =
        DependencyProperty.Register(nameof(ScreenSize),
                                    typeof(Size<int>),
                                    typeof(ViewportRegionImagePresenter),
                                    new PropertyMetadata(new Size<int>(1, 1)));

    public float ZoomMultiplier
    {
        get => (float)GetValue(ZoomMultiplierProperty);
        set => SetValue(ZoomMultiplierProperty, value);
    }

    public static readonly DependencyProperty ZoomMultiplierProperty =
        DependencyProperty.Register(nameof(ZoomMultiplier),
                                    typeof(float),
                                    typeof(ViewportRegionImagePresenter),
                                    new PropertyMetadata(1f));
}