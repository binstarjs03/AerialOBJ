using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using binstarjs03.AerialOBJ.Core.CoordinateSystem;
using binstarjs03.AerialOBJ.Core.WorldRegion;
using binstarjs03.AerialOBJ.WpfApp.Converters;

using Image = System.Windows.Controls.Image;

using PointInt2 = binstarjs03.AerialOBJ.Core.CoordinateSystem.PointInt2;
using Section = binstarjs03.AerialOBJ.Core.WorldRegion.Section;

namespace binstarjs03.AerialOBJ.WpfApp.UIElements.Components;

public class ChunkImage : Image, IDisposable
{
    private static readonly PixelFormat s_format = PixelFormats.Bgra32;
    private static readonly int s_bitsPerByte = 8;
    private static readonly int s_bytesPerPixel = s_format.BitsPerPixel / s_bitsPerByte;

    private readonly Chunk _chunk;
    private readonly Coords2 _pos;
    private WriteableBitmap _buff;

    private bool _disposed;

    public ChunkImage(Chunk chunk)
    {
        RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.NearestNeighbor);
        RenderOptions.SetEdgeMode(this, EdgeMode.Aliased);
        _buff = new WriteableBitmap(16, 16, 96, 96, s_format, palette: null);
        Stretch = Stretch.UniformToFill;
        Source = _buff;
        _pos = chunk.CoordsAbs;
        _chunk = chunk;
    }

    public Coords2 Pos => _pos;
    public PointInt2 CanvasPos => (PointInt2)_pos;

    public void SetImageToChunkTerrain()
    {
        try
        {
            _buff.Lock();
            Block[,] blocks = _chunk.GetBlockTopmost(new string[] { "minecraft:air" });
            for (int x = 0; x < Section.BlockCount; x++)
            {
                for (int z = 0; z < Section.BlockCount; z++)
                {
                    Coords2 coords = new(x, z);
                    Color color = BlockToColor.Convert(blocks[x,z]);
                    SetBlockColor((PointInt2)coords, color);
                }
            }
        }
        finally
        {
            _buff.Unlock();
        }
    }

    private void SetBlockColor(PointInt2 blockPixelPos, Color color)
    {
        //if (pixelPos.X > 15)
        int xOffset = blockPixelPos.X * s_bytesPerPixel;
        int yOffset = blockPixelPos.Y * s_bytesPerPixel * _buff.PixelWidth;
        int offset = xOffset + yOffset;
        IntPtr backBuffPtr = _buff.BackBuffer;
        unsafe
        {
            byte* backBuffData = (byte*)backBuffPtr.ToPointer();
            backBuffData[offset + 0] = color.B;
            backBuffData[offset + 1] = color.G;
            backBuffData[offset + 2] = color.R;
            backBuffData[offset + 3] = color.A;
        }

        /* Mark buffer area of mutated pixel
           so the front buffer will be updated */
        _buff.AddDirtyRect(new(blockPixelPos.X, blockPixelPos.Y, 1, 1));
    }

    #region Dispose Pattern

#   pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
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
            _buff = null;
            _disposed = true;
        }
    }
#   pragma warning restore CS8625

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
