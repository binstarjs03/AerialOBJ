using System;

using binstarjs03.AerialOBJ.Core;
using binstarjs03.AerialOBJ.Core.Primitives;
using CommunityToolkit.Mvvm.ComponentModel;

namespace binstarjs03.AerialOBJ.MVVM.ViewModels.Viewport;
public partial class ViewportViewModel : ObservableObject
{
    private readonly float[] _zoomTable = new float[] {
        1, 2, 3, 5, 8, 13, 21, 34 // fib. sequence
    }; 
    private readonly float _zoomLowLimit = 1f;
    private readonly float _zoomHighLimit = 32f;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SelectionStart))]
    [NotifyPropertyChangedFor(nameof(SelectionSize))]
    private PointZ<int> _selection1;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SelectionStart))]
    [NotifyPropertyChangedFor(nameof(SelectionSize))]
    private PointZ<int> _selection2;

    public ViewportViewModel(IViewportInfo viewportInfo, ViewportRegionImageViewModel regionImageViewModel)
    {
        ViewportInfo = viewportInfo;
        RegionImageViewModel = regionImageViewModel;
    }

    public PointZ<int> SelectionStart => new(Math.Min(Selection1.X, Selection2.X), 
                                             Math.Min(Selection1.Z, Selection2.Z));
    public Size<int> SelectionSize => new(
        (Math.Abs(Selection2.X - Selection1.X) * ViewportInfo.ZoomMultiplier).Ceiling(), 
        (Math.Abs(Selection2.Z - Selection1.Z) * ViewportInfo.ZoomMultiplier).Ceiling());

    public IViewportInfo ViewportInfo { get; }
    public ViewportRegionImageViewModel RegionImageViewModel { get; }
}
