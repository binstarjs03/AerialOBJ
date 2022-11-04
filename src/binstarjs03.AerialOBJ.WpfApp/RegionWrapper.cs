using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

using binstarjs03.AerialOBJ.Core.CoordinateSystem;
using binstarjs03.AerialOBJ.Core.MinecraftWorld;
using binstarjs03.AerialOBJ.WpfApp.Converters;
using binstarjs03.AerialOBJ.WpfApp.UIElements.Components;
using binstarjs03.AerialOBJ.WpfApp.UIElements.Controls;

namespace binstarjs03.AerialOBJ.WpfApp;

public class RegionWrapper
{
    private readonly Region _region;
    private readonly RegionImage _regionImage;
    private readonly ViewportControlVM _viewport;
    private readonly Coords2 _regionCoords;
    private readonly HashSet<Coords2> _generatedChunks;

    public RegionImage RegionImage => _regionImage;
    public Coords2 RegionCoords => _regionCoords;

    public RegionWrapper(Region region, ViewportControlVM viewport)
    {
        _region = region;
        _regionImage = App.CurrentCast.Dispatcher.Invoke(
            () => new RegionImage(),
            DispatcherPriority.Render);
        _viewport = viewport;
        _regionCoords = region.RegionCoords;
        (_, _generatedChunks) = region.GetGeneratedChunksAsCoordsRel();
    }

    public Chunk GetChunk(Coords2 chunkCoords, bool relative)
    {
        return _region.GetChunk(chunkCoords, relative);
    }

    public bool HasChunkGenerated(Coords2 chunkCoordsRel)
    {
        return _generatedChunks.Contains(chunkCoordsRel);
    }

    public void AddOrUpdateChunkImage(Coords2 chunkCoordsRel, string[,] highestBlocks)
    {
        for (int x = 0; x < Section.BlockCount; x++)
            for (int z = 0; z < Section.BlockCount; z++)
            {
                Color color = BlockToColor.Convert(highestBlocks[x, z]);
                _regionImage.SetPixel(chunkCoordsRel.X * Section.BlockCount + x,
                                      chunkCoordsRel.Z * Section.BlockCount + z,
                                      color);
            }
    }

    public void RemoveChunkImage(Coords2 chunkCoordsRel)
    {
        for (int x = 0; x < Section.BlockCount; x++)
            for (int z = 0; z < Section.BlockCount; z++)
            {
                _regionImage.SetPixel(chunkCoordsRel.X * Section.BlockCount + x,
                                      chunkCoordsRel.Z * Section.BlockCount + z,
                                      Colors.Transparent);
            }
    }

    public void UpdateImageTransformation()
    {
        UpdateImagePosition();
        UpdateImageSize();
    }

    private void UpdateImagePosition()
    {
        // we floor all the floating-point number here
        // so it snaps perfectly to the pixel and it removes
        // "Jaggy-Moving" illusion.
        // Try to not floor it and see yourself the illusion

        // scaled unit is offset amount required to align the
        // coordinate to zoomed coordinate measured from world origin.
        // Here we are scaling the cartesian coordinate unit by zoom amount
        // (which is pixel-per-chunk)

        PointInt2 scaledUnit = (PointInt2)_region.RegionCoords * _viewport.ViewportPixelPerRegion;

        // Push toward center is offset amount required to align the coordinate
        // relative to the canvas center,
        // so it creates "zoom toward center" effect

        PointF2 pushTowardCenter = _viewport.ViewportChunkCanvasCenter;

        // Origin offset is offset amount requred to align the coordinate
        // to keep it stays aligned with moved world origin
        // when view is dragged around.
        // The offset itself is from camera position.
        // It is inverted because obviously, if camera is 1 meter to the right
        // of origin, then everything else the camera sees must be 1 meter
        // shifted to the left of the camera

        PointF2 originOffset = -_viewport.RegionPosOffset;

        PointF2 finalPos
            = (originOffset + scaledUnit + pushTowardCenter).Floor;
        Canvas.SetLeft(_regionImage.Image, finalPos.X);
        Canvas.SetTop(_regionImage.Image, finalPos.Y);
    }

    private void UpdateImageSize()
    {
        _regionImage.Image.Width = _viewport.ViewportPixelPerRegion;
    }
}
