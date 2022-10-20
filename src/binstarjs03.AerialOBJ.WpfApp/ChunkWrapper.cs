using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

using binstarjs03.AerialOBJ.Core.CoordinateSystem;
using binstarjs03.AerialOBJ.Core.WorldRegion;
using binstarjs03.AerialOBJ.WpfApp.UIElements.Components;
using binstarjs03.AerialOBJ.WpfApp.UIElements.Controls;

namespace binstarjs03.AerialOBJ.WpfApp;

public class ChunkWrapper
{
    private readonly ChunkManager _manager;
    private readonly ViewportControlVM _viewport;
    private readonly Coords2 _pos;
    private ChunkImage? _chunk;
    private bool _abortAllocation = false;

    public ChunkWrapper(Coords2 pos, ChunkManager manager)
    {
        _pos = pos;
        _manager = manager;
        _viewport = manager.Viewport;
    }

    public void Allocate()
    {
        // cancel allocation if chunk is outside screen frame
        if (!_manager.VisibleChunkRange.IsInside(_pos))
            return;
        Application.Current.Dispatcher.BeginInvoke(
            method: OnAllocate,
            DispatcherPriority.ContextIdle);
    }

    private void OnAllocate()
    {
        if (_abortAllocation)
            return;
        Chunk? chunk = _manager.RegionManager.GetChunk(_pos);
        if (chunk is null)
            return;
        _chunk = new ChunkImage(chunk);

        Task.Run(() =>
        {
            // at any time when this invoked, chunk may be null or aborted
            // this can happened when the thread that
            // executing this is happening after chunk is deallocated (too late to execute)
            if (_abortAllocation || _chunk is null)
                return;
            _chunk.SetImageToChunkTerrain();
        });

        _viewport.Control.ChunkCanvas.Children.Add(_chunk);
        OnReallocated();
    }

    public void Deallocate()
    {
        _abortAllocation = true;
        if (_chunk is null)
            return;
        _viewport.Control.ChunkCanvas.Children.Remove(_chunk);
        OnReallocated();
        _chunk.Dispose();
        _chunk = null;
    }

    private void OnReallocated()
    {
        _viewport.NotifyPropertyChanged(nameof(_viewport.ChunkManagerLoadedChunkCount));
        _viewport.NotifyPropertyChanged(nameof(_viewport.ChunkManagerPendingChunkCount));
    }

    public void Update()
    {
        if (_chunk is null)
            return;
        UpdatePosition(_chunk);
        UpdateSize(_chunk);

        void UpdatePosition(ChunkImage chunk)
        {
            // we floor all the floating-point number here
            // so it snaps perfectly to the pixel and it removes
            // "Jaggy-Moving" illusion.
            // Try to not floor it and see yourself the illusion

            PointF2 chunkPosOffset = _viewport.ChunkPosOffset.Floor;

            // scaled unit is offset amount required to align the
            // coordinate to zoomed coordinate measured from world origin.
            // Here we are scaling the cartesian coordinate unit by zoom amount
            // (which is pixel-per-chunk)

            PointInt2 scaledUnit = chunk.CanvasPos * _viewport.ViewportPixelPerChunk;

            // Push toward center is offset amount required to align the coordinate
            // relative to the chunk canvas center,
            // so it creates "zoom toward center" effect

            PointF2 pushTowardCenter = _viewport.ViewportChunkCanvasCenter.Floor;

            // Origin offset is offset amount requred to align the coordinate
            // to keep it stays aligned with moved world origin
            // when view is dragged around.
            // The offset itself is from camera position.
            // It is inverted because obviously, if camera is 1 meter to the right
            // of origin, then everything else the camera sees must be 1 meter
            // shifted to the left of the camera

            PointF2 originOffset = -chunkPosOffset;

            PointF2 finalPos
                = (originOffset + scaledUnit + pushTowardCenter).Floor;
            Canvas.SetLeft(chunk, finalPos.X);
            Canvas.SetTop(chunk, finalPos.Y);
        }

        void UpdateSize(ChunkImage chunk)
        {
            chunk.Width = _viewport.ViewportPixelPerChunk;
        }
    }
}
