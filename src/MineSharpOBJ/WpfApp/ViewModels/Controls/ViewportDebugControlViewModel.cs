using System.ComponentModel;

using binstarjs03.MineSharpOBJ.WpfApp.ViewModels.Windows;
using binstarjs03.MineSharpOBJ.WpfApp.Views.Controls;
namespace binstarjs03.MineSharpOBJ.WpfApp.ViewModels.Controls;

public class ViewportDebugControlViewModel : ViewModelBase<ViewportDebugControlViewModel, ViewportDebugControl> {
    private string _viewportCameraPos = "";
    private string _viewportCameraPosZoomed = "";
    private string _viewportZoomLevel = "";
    private string _viewportPixelPerBlock = "";
    private string _viewportPixelPerChunk = "";

    private string _mousePos = "";
    private string _mousePosDelta = "";
    private string _mouseIsClickHolding = "";
    private string _mouseIsOutside = "";

    public ViewportDebugControlViewModel(ViewportDebugControl control) : base(control) {
        // initialize states
        IsVisible = false;
        ReinitializeText();
    }

    // States -----------------------------------------------------------------

    public string CameraPos {
        get { return _viewportCameraPos; }
        set {
            _viewportCameraPos = value;
            OnPropertyChanged(nameof(CameraPos));
        }
    }

    public string CameraPosZoomed {
        get { return _viewportCameraPosZoomed; }
        set {
            _viewportCameraPosZoomed = value;
            OnPropertyChanged(nameof(CameraPosZoomed));
        }
    }

    public string ZoomLevel {
        get { return _viewportZoomLevel; }
        set {
            _viewportZoomLevel = value;
            OnPropertyChanged(nameof(ZoomLevel));
        }
    }

    public string PixelPerBlock {
        get { return _viewportPixelPerBlock; }
        set {
            _viewportPixelPerBlock = value;
            OnPropertyChanged(nameof(PixelPerBlock));
        }
    }

    public string PixelPerChunk {
        get { return _viewportPixelPerChunk; }
        set {
            _viewportPixelPerChunk = value;
            OnPropertyChanged(nameof(PixelPerChunk));
        }
    }

    public string MousePos {
        get { return _mousePos; }
        set {
            _mousePos = value;
            OnPropertyChanged(nameof(MousePos));
        }
    }

    public string MousePosDelta {
        get { return _mousePosDelta; }
        set {
            _mousePosDelta = value;
            OnPropertyChanged(nameof(MousePosDelta));
        }
    }

    public string MouseIsClickHolding {
        get { return _mouseIsClickHolding; }
        set {
            _mouseIsClickHolding = value;
            OnPropertyChanged(nameof(MouseIsClickHolding));
        }
    }

    public string MouseIsOutside {
        get { return _mouseIsOutside; }
        set {
            _mouseIsOutside = value;
            OnPropertyChanged(nameof(MouseIsOutside));
        }
    }

    // Private Methods --------------------------------------------------------

    private void ReinitializeText() {
        _viewportCameraPos = "Camera Pos:";
        _viewportCameraPosZoomed = "Camera Pos (zoomed):";
        _viewportZoomLevel = "Zoom Level:";
        _viewportPixelPerBlock = "Pixel-Per-Block:";
        _viewportPixelPerChunk = "Pixel-Per-Chunk:";

        _mousePos = "Mouse Pos:";
        _mousePosDelta = "Mouse Pos Delta:";
        _mouseIsClickHolding = "Is Click Holding:";
        _mouseIsOutside = "Is Outside:";
    }
}

