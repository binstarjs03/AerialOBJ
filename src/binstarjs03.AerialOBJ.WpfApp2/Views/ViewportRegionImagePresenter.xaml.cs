using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

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
}