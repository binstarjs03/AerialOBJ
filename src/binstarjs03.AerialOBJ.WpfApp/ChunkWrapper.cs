using System.IO;
using System.Windows.Controls;

using binstarjs03.AerialOBJ.Core.CoordinateSystem;
using binstarjs03.AerialOBJ.WpfApp.UIElements.Components;
using binstarjs03.AerialOBJ.WpfApp.UIElements.Controls;

namespace binstarjs03.AerialOBJ.WpfApp;

public class ChunkWrapper
{
    private readonly ChunkManager _chunkManager;
    private readonly ViewportControlVM _viewport;
    private readonly Coords2 _chunkCoordsAbs;
    private ChunkImage _chunkImage;

    public ChunkImage ChunkImage => _chunkImage;
    public Coords2 ChunkCoordsAbs => _chunkCoordsAbs;

    public ChunkWrapper(Coords2 chunkCoordsAbs, ChunkManager chunkManager, MemoryStream chunkImageStream)
    {
        _chunkCoordsAbs = chunkCoordsAbs;
        _chunkManager = chunkManager;
        _viewport = chunkManager.Viewport;
        _chunkImage = new(_chunkCoordsAbs);
        _chunkImage.SetImageToChunkTerrain(chunkImageStream);
        Update();
    }

    public void Deallocate()
    {
        _chunkImage.Dispose();
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
