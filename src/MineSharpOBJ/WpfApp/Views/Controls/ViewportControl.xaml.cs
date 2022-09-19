using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using binstarjs03.MineSharpOBJ.Core.Utils;
using binstarjs03.MineSharpOBJ.WpfApp.ViewModels.Controls;

namespace binstarjs03.MineSharpOBJ.WpfApp.Views.Controls;

// TODO create IViewModel interface that has ViewModel properties,
// which returns the underlying VM
public partial class ViewportControl : UserControl {
    private readonly ViewportControlViewModel _vm;

    public ViewportControl() {
        InitializeComponent();
        _vm = new ViewportControlViewModel(this);
        ViewportControlViewModel.Context = _vm;
        DataContext = _vm;
    }

    // TODO move all below back-end / business logic to VM
    // as View roles is just showing the UI, not handling logic
    private void OnMouseWheel(object sender, MouseWheelEventArgs e) {
        if (e.Delta > 0)
            _vm.ZoomLevel++;
        else
            _vm.ZoomLevel--;
        //if (DebugPanelControlViewModel.Context is not null)
        //    DebugPanelControlViewModel.Context.ZoomLevel = $"Zoom Level: {_zoomLevel}";
        _vm.UpdateChunkTransformation(updateChunkSize: true);
    }
    private void OnSizeChanged(object sender, SizeChangedEventArgs e) {
        // we don't update chunk size because we are not zooming,
        // we are just handling what happen when window is resized
        _vm.UpdateChunkTransformation(updateChunkSize: false);
    }

    private void OnMouseMove(object sender, MouseEventArgs e) {
        // set delta to 0 if this call is initial click and dragging.
        // this is to avoid jumps when clicking menu bar
        // then clicking and dragging on the viewer again
        PointF mousePosNew = new(e.GetPosition(this).X,
                              e.GetPosition(this).Y);
        PointF mousePosOld = _vm.MousePos;
        PointF mousePosDelta = _vm.MousePosDelta;

        mousePosDelta.X = _vm.InitClickDrag ? 0 : mousePosNew.X - mousePosOld.X;
        mousePosDelta.Y = _vm.InitClickDrag ? 0 : mousePosNew.Y - mousePosOld.Y;
        
        _vm.MousePos = mousePosNew;
        _vm.MousePosDelta = mousePosDelta;
        //if (DebugPanelControlViewModel.Context is not null)
        //    DebugPanelControlViewModel.Context.MousePos = $"MousePos: ({mousePosNew.X}, {mousePosNew.Y})";

        if (_vm.IsClickHolding) {
            _vm.CameraPos += (_vm.MousePosDelta / _vm.PixelPerBlock);
            _vm.InitClickDrag = false;
            _vm.UpdateChunkTransformation(updateChunkSize: false);
        }
    }

    private void OnMouseUp(object sender, MouseButtonEventArgs e) {
        if (e.LeftButton == MouseButtonState.Released) {
            _vm.IsClickHolding = false;
            _vm.InitClickDrag = true;
        }
    }

    private void OnMouseDown(object sender, MouseButtonEventArgs e) {
        if (e.LeftButton == MouseButtonState.Pressed) {
            _vm.IsClickHolding = true;
        }
    }
}
