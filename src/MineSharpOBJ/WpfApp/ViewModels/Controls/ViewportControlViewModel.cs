using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

using binstarjs03.MineSharpOBJ.Core.RegionMc;
using binstarjs03.MineSharpOBJ.Core.Utils;
using binstarjs03.MineSharpOBJ.WpfApp.Models;
using binstarjs03.MineSharpOBJ.WpfApp.Views.Controls;

namespace binstarjs03.MineSharpOBJ.WpfApp.ViewModels.Controls;

public class ViewportControlViewModel : ViewModelBase<ViewportControlViewModel, ViewportControl> {
    private static readonly int[] s_zoomBlockPixelCount = new int[] {
        1, 2, 3, 5, 8, 13, 21, 34
    };
    private int _zoomLevel = 2;
    private bool _isClickHolding = false;
    private PointF _cameraPos = PointF.Origin;

    private bool _initClickDrag = true;
    private PointF _mousePos = PointF.Origin;
    private PointF _mousePosDelta = PointF.Origin;

    public ViewportControlViewModel(ViewportControl viewport) : base(viewport) {
        // TODO delete all of below codes once chunk loader is implemented stable
        for (int x = -8; x < 16; x++) {
            for (int y = -8; y < 16; y++) {
                ChunkModel chunk = new(new(x, y));
                chunk.SetRandomImage((x + y) % 2 == 0);
                Control.ChunkCanvas.Children.Add(chunk);
                chunk.Width = PixelPerChunk;
            }
        }
    }

    // States -----------------------------------------------------------------

    public int ZoomLevel {
        get { return _zoomLevel; }
        set {
            if (value == _zoomLevel)
                return;
            _zoomLevel = Math.Clamp(value, 1, s_zoomBlockPixelCount.Length);
            OnPropertyChanged(nameof(ZoomLevel));
        }
    }

    public bool IsClickHolding {
        get { return _isClickHolding; }
        set {
            if (value == _isClickHolding)
                return;
            _isClickHolding = value;
            OnPropertyChanged(nameof(IsClickHolding));
        }
    }

    public PointF CameraPos {
        get { return _cameraPos; }
        set {
            if (value == _cameraPos)
                return;
            _cameraPos = value;
            OnPropertyChanged(nameof(CameraPos));
        }
    }

    public bool InitClickDrag {
        get { return _initClickDrag; }
        set {
            if (value == _initClickDrag)
                return;
            _initClickDrag = value;
            OnPropertyChanged(nameof(InitClickDrag));
        }
    }

    public PointF MousePos {
        get { return _mousePos; }
        set {
            if (value == _mousePos)
                return;
            _mousePos = value;
            OnPropertyChanged(nameof(MousePos));
        }
    }

    public PointF MousePosDelta {
        get { return _mousePosDelta; }
        set {
            if (value == _mousePosDelta)
                return;
            _mousePosDelta = value;
            OnPropertyChanged(nameof(MousePosDelta));
        }
    }

    public int PixelPerBlock => s_zoomBlockPixelCount[_zoomLevel - 1];

    public int PixelPerChunk => PixelPerBlock * Section.BlockCount;

    // Methods ----------------------------------------------------------------

    public void UpdateCameraPosition(PointF cameraPos) {
        _cameraPos = cameraPos;
        UpdateChunkTransformation(updateChunkSize: false);
    }

    public void UpdateChunkTransformation(bool updateChunkSize) {
        PointF screenCenterPoint = new(
            Control.ActualWidth / 2,
            Control.ActualHeight / 2
        );
        foreach (ChunkModel chunk in Control.ChunkCanvas.Children) {
            if (updateChunkSize)
                UpdateChunkSize(chunk);
            UpdateChunkPosition(chunk, screenCenterPoint);
        }
    }

    public void UpdateChunkSize(ChunkModel chunk) {
        chunk.Width = PixelPerChunk;
    }

    public void UpdateChunkPosition(ChunkModel chunk, PointF centerPoint) {
        PointF cameraPosZoomed = _cameraPos * PixelPerBlock;
        //if (DebugPanelControlViewModel.Context is not null) {
        //    DebugPanelControlViewModel.Context.CameraPos
        //        = $"ViewPos: ({Math.Round(_cameraPos.X, 3)}, {Math.Round(_cameraPos.Y, 3)})";
        //    DebugPanelControlViewModel.Context.PPC = $"PPC: {ChunkPixelCount}";
        //}

        // Push origin is offset amount required to align the coordinate
        // to zoomed coordinate measured from world origin
        Point scaleFromWorldOrigin = new(
            PixelPerChunk * chunk.CanvasPos.X,
            PixelPerChunk * chunk.CanvasPos.Y
        );

        // Push toward center is offset amount required to align the coordinate
        // relative to the screen (more precisely, this ViewportControl) center
        // so it creates "zoom toward center" effect
        PointF pushTowardCenter = centerPoint;

        // Origin offset is offset amount requred to align the coordinate
        // to keep it stays aligned with moved world origin
        // when view is dragged around.
        // The offset itself is from camera position.
        PointF originOffset = cameraPosZoomed;

        PointF finalPos = scaleFromWorldOrigin + originOffset + pushTowardCenter;
        Canvas.SetLeft(chunk, finalPos.X);
        Canvas.SetTop(chunk, finalPos.Y);
    }
}
