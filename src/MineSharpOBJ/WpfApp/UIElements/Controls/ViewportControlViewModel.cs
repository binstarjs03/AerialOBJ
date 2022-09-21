using System.ComponentModel;

using binstarjs03.MineSharpOBJ.WpfApp.UIElements.Windows;

namespace binstarjs03.MineSharpOBJ.WpfApp.UIElements.Controls;

public class ViewportControlViewModel : ViewModelBase<ViewportControlViewModel, ViewportControl> {
    private bool _isCameraPositionGuideVisible = false;

    public ViewportControlViewModel(ViewportControl control) : base(control) {
        //IsCameraPositionGuideVisible = MainWindowViewModel.Context!.IsViewportCameraPositionGuideVisible;
    }

    public override void StartEventListening() {
        MainWindowViewModel.Context!.PropertyChanged += OnOtherViewModelPropertyChanged;
    }

    // States -----------------------------------------------------------------

    public bool IsCameraPositionGuideVisible {
        get { return _isCameraPositionGuideVisible; }
        set {
            if (value == _isCameraPositionGuideVisible)
                return;
            _isCameraPositionGuideVisible = value;
            OnPropertyChanged(nameof(IsCameraPositionGuideVisible));
        }
    }

    // Event Handlers ---------------------------------------------------------

    protected override void OnOtherViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e) {
        if (sender is MainWindowViewModel vm) {
            if (e.PropertyName == nameof(MainWindowViewModel.IsViewportDebugControlVisible)) {
                ViewportDebugControlViewModel.Context!.IsVisible = vm.IsViewportDebugControlVisible;
            }
            else if (e.PropertyName == nameof(MainWindowViewModel.IsViewportCameraPositionGuideVisible)) {
                IsCameraPositionGuideVisible = vm.IsViewportCameraPositionGuideVisible;
            }
        }
    }
}
