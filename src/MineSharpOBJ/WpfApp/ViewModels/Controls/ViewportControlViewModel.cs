using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using binstarjs03.MineSharpOBJ.WpfApp.ViewModels.Windows;
using binstarjs03.MineSharpOBJ.WpfApp.Views.Controls;
namespace binstarjs03.MineSharpOBJ.WpfApp.ViewModels.Controls;

public class ViewportControlViewModel : ViewModelBase<ViewportControlViewModel, ViewportControl> {
    private bool _isCameraPositionGuideVisible = false;

    public ViewportControlViewModel(ViewportControl control) : base(control) {
        // Bin: I decided to comment out this because the designer sees it as NullReference 
        // to main window vm context
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
