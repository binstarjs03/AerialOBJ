using CommunityToolkit.Mvvm.ComponentModel;

namespace binstarjs03.AerialOBJ.WpfApp.ViewModels;

[ObservableObject]
public partial class GotoViewModel
{
    private int _xfield = 0;
    private int _zfield = 0;

    public GotoViewModel(ViewportViewModel viewportViewModel)
    {
        ViewportViewModel = viewportViewModel;
    }

    public int XField
    {
        get => _xfield;
        set
        {
            if (_xfield == value)
                return;
            _xfield = value;
            OnPropertyChanged();
            var cameraPos = ViewportViewModel.CameraPos;
            cameraPos.X = _xfield;
            ViewportViewModel.CameraPos = cameraPos;
        }
    }

    public int ZField
    {
        get => _zfield;
        set
        {
            if (_zfield == value)
                return;
            _zfield = value;
            OnPropertyChanged();
            var cameraPos = ViewportViewModel.CameraPos;
            cameraPos.Z = _zfield;
            ViewportViewModel.CameraPos = cameraPos;
        }
    }

    public ViewportViewModel ViewportViewModel { get; }

    private void OnCurrent()
    {

    }
}
