using System;
using System.Drawing;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using binstarjs03.AerialOBJ.Core.CoordinateSystem;
using binstarjs03.AerialOBJ.Core.WorldRegion;
using binstarjs03.AerialOBJ.WpfApp.Converters;

using ImageFormat = System.Drawing.Imaging.ImageFormat;
using Image = System.Windows.Controls.Image;

using PointInt2 = binstarjs03.AerialOBJ.Core.CoordinateSystem.PointInt2;
using Section = binstarjs03.AerialOBJ.Core.WorldRegion.Section;

namespace binstarjs03.AerialOBJ.WpfApp.UIElements.Components;

public class ChunkImage : Image, IDisposable
{
    private readonly Chunk _chunk;
    private readonly Coords2 _pos;

    private bool _disposed;

    public ChunkImage(Chunk chunk)
    {
        RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.NearestNeighbor);
        RenderOptions.SetEdgeMode(this, EdgeMode.Aliased);
        Stretch = Stretch.UniformToFill;
        _pos = chunk.CoordsAbs;
        _chunk = chunk;
    }

    public Coords2 Pos => _pos;
    public PointInt2 CanvasPos => (PointInt2)_pos;

    // call this from secondary thread as calling it from main thread will
    // block UI thread!
    public void SetImageToChunkTerrain()
    {
        Bitmap bitmap = new(16, 16);
        Block[,] blocks = _chunk.GetBlockTopmost(new string[] { "minecraft:air" });
        for (int x = 0; x < Section.BlockCount; x++)
        {
            for (int z = 0; z < Section.BlockCount; z++)
            {
                bitmap.SetPixel(x, z, BlockToColor2.Convert(blocks[x, z]));
            }
        }
        MemoryStream memory = new();
        bitmap.Save(memory, ImageFormat.Bmp);
        memory.Position = 0;

        object[] args = new object[]
        {
            memory
        };

        Action<object> method = (object arg) =>
        {
            MemoryStream memory = (MemoryStream)arg;
            BitmapImage image = new();
            image.BeginInit();
            image.StreamSource = memory;
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.EndInit();
            Source = image;
            memory.Dispose();
        };

        Dispatcher.BeginInvoke(method, System.Windows.Threading.DispatcherPriority.Background, args);
    }

    #region Dispose Pattern

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                // dispose managed state (managed objects)
            }

            // free unmanaged resources (unmanaged objects) and override finalizer
            // set large fields to null
            Source = null;
            _disposed = true;
        }
    }

    // // override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~ChunkControl()
    // {
    //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    //     Dispose(disposing: false);
    // }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    #endregion
}
