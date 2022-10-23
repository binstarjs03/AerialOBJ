using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

using binstarjs03.AerialOBJ.Core.CoordinateSystem;
using binstarjs03.AerialOBJ.Core.WorldRegion;
using binstarjs03.AerialOBJ.WpfApp.Converters;
using binstarjs03.AerialOBJ.WpfApp.UIElements.Components;
using binstarjs03.AerialOBJ.WpfApp.UIElements.Controls;

namespace binstarjs03.AerialOBJ.WpfApp;

public class ChunkWrapper
{
    private readonly ChunkManager _manager;
    private readonly ViewportControlVM _viewport;
    private readonly Coords2 _pos;
    private ChunkImage? _chunkImage;
    private bool _abortAllocation = false;

    public ChunkWrapper(Coords2 pos, ChunkManager manager)
    {
        _pos = pos;
        _manager = manager;
        _viewport = manager.Viewport;
    }

    // return true if can allocate, false otherwise
    public bool Allocate(int heightLimit)
    {
        // cancel allocation if chunk is outside screen frame
        if (!_manager.VisibleChunkRange.IsInside(_pos))
            return false;
        // cancel allocation if returned chunk does not exist
        if (_manager.RegionManager.CanGetChunk(_pos))
        {
            Task.Run(() => OnAllocateThreaded(heightLimit));
            return true;
        }
        else
        {
            _abortAllocation = true;
            return false;
        }
    }

    private void OnAllocateThreaded(int heightLimit)
    {
        // cancel allocation if chunk is outside screen frame
        if (!_manager.VisibleChunkRange.IsInside(_pos))
            return;
        if (_abortAllocation)
            return;
        Chunk? chunk = _manager.RegionManager.GetChunk(_pos);
        if (chunk is null)
            return;

        Bitmap bitmap = new(16, 16, PixelFormat.Format32bppArgb);
        Block[,] blocks = new Block[Section.BlockCount, Section.BlockCount];
        chunk.GetBlockTopmost(blocks, heightLimit: heightLimit);
        for (int x = 0; x < Section.BlockCount; x++)
        {
            for (int z = 0; z < Section.BlockCount; z++)
            {
                if (_abortAllocation)
                    return;
                if (!_manager.VisibleChunkRange.IsInside(_pos))
                    return;
                bitmap.SetPixel(x, z, BlockToColor2.Convert(blocks[x, z]));
            }
        }
        MemoryStream memory = new();
        bitmap.Save(memory, ImageFormat.Bmp);

        // at this point application may be already terminated, we just return it
        if (Application.Current is null)
            return;
        Application.Current.Dispatcher.BeginInvoke(
            method: OnAllocateDispatcher,
            DispatcherPriority.Background,
            new object[] { memory });
    }

    private void OnAllocateDispatcher(MemoryStream memory)
    {
        _chunkImage = new(_pos);
        _chunkImage.SetImageToChunkTerrain(memory);
        Update();
        if (_abortAllocation)
            return;
        _viewport.Control.ChunkCanvas.Children.Add(_chunkImage);
        OnReallocated();
    }

    public void Deallocate()
    {
        _abortAllocation = true;
        if (_chunkImage is null)
            return;
        _viewport.Control.ChunkCanvas.Children.Remove(_chunkImage);
        OnReallocated();
        _chunkImage.Dispose();
        _chunkImage = null;
    }

    private void OnReallocated()
    {
        _viewport.NotifyPropertyChanged(nameof(_viewport.ChunkManagerLoadedChunkCount));
        _viewport.NotifyPropertyChanged(nameof(_viewport.ChunkManagerPendingChunkCount));
    }

    public void Update()
    {
        if (_chunkImage is null)
            return;
        UpdatePosition(_chunkImage);
        UpdateSize(_chunkImage);

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
