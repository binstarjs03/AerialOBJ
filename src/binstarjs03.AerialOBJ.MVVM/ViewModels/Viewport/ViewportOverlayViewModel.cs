using CommunityToolkit.Mvvm.ComponentModel;

namespace binstarjs03.AerialOBJ.MVVM.ViewModels.Viewport;

// TODO this class blurs the line whether this is a viewmodel or an interface of state
public partial class ViewportOverlayViewModel : ObservableObject
{
    private readonly IViewportInfo _viewportInfo;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsRegionCoordsTextVisible))]
    private bool _isChunkGridVisible = false;

    [ObservableProperty] 
    private bool _isRegionGridVisible = false;

    [ObservableProperty] 
    private bool _isInfoPanelVisible = false;

    [ObservableProperty] 
    private bool _isCameraPositionVisile = false;

    public ViewportOverlayViewModel(IViewportInfo viewportInfo)
    {
        _viewportInfo = viewportInfo;
        _viewportInfo.ZoomMultiplierChanged += _ => OnPropertyChanged(nameof(IsRegionCoordsTextVisible));
    }

    public bool IsRegionCoordsTextVisible => _viewportInfo.ZoomMultiplier >= 1f 
                                          && _viewportInfo.ZoomMultiplier <= 2f 
                                          && IsChunkGridVisible;
}
