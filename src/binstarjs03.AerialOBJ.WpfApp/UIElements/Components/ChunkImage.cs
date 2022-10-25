using System;
using System.Drawing;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using binstarjs03.AerialOBJ.Core.CoordinateSystem;

using ImageFormat = System.Drawing.Imaging.ImageFormat;
using Image = System.Windows.Controls.Image;

using PointInt2 = binstarjs03.AerialOBJ.Core.CoordinateSystem.PointInt2;

namespace binstarjs03.AerialOBJ.WpfApp.UIElements.Components;

public class ChunkImage : Image, IDisposable
{
    private readonly Coords2 _chunkCoordsAbs;
    private bool _disposed;

    public ChunkImage(Coords2 chunkCoordsAbs)
    {
        RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.NearestNeighbor);
        RenderOptions.SetEdgeMode(this, EdgeMode.Aliased);
        Stretch = Stretch.UniformToFill;
        _chunkCoordsAbs = chunkCoordsAbs;
    }

    public Coords2 ChunkCoordsAbs => _chunkCoordsAbs;
    public PointInt2 CanvasPos => (PointInt2)_chunkCoordsAbs;

    // call this from secondary thread as calling it from main thread will
    // block UI thread!
    public void SetImageToChunkTerrain(MemoryStream chunkImageStream)
    {
        chunkImageStream.Position = 0;

        BitmapImage image = new();
        image.BeginInit();
        image.StreamSource = chunkImageStream;
        image.CacheOption = BitmapCacheOption.OnLoad;
        image.EndInit();
        image.Freeze();
        Source = image;
        chunkImageStream.Dispose();

    }

    #region Dispose Pattern

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            Source = null;
            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    #endregion
}
