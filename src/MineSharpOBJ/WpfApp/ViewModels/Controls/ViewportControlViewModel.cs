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
    public ViewportControlViewModel(ViewportControl control) : base(control) { }

    public override void StartEventListening() {
        MainWindowViewModel.Context!.PropertyChanged += OnOtherViewModelPropertyChanged;
    }

    // Event Handlers ---------------------------------------------------------

    protected override void OnOtherViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e) {
        if (sender is MainWindowViewModel vm) {
            if (e.PropertyName == nameof(MainWindowViewModel.IsViewportDebugControlVisible)) {
                ViewportDebugControlViewModel.Context!.IsVisible = vm.IsViewportDebugControlVisible;
            }
        }
    }
}
