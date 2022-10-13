using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using Color = System.Drawing.Color;
using Image = System.Windows.Controls.Image;

using PointInt2 = binstarjs03.CubeOBJ.Core.CoordinateSystem.PointInt2;
using Section = binstarjs03.CubeOBJ.Core.WorldRegion.Section;

namespace binstarjs03.CubeOBJ.WpfApp.UIElements.Controls;

public class ChunkControl : Image, IDisposable
{
    private static readonly PixelFormat s_format = PixelFormats.Bgra32;
    private static readonly int s_bitsPerByte = 8;
    private static readonly int s_bytesPerPixel = s_format.BitsPerPixel / s_bitsPerByte;

    private readonly PointInt2 _canvasPos;
    private WriteableBitmap _buff;

    private bool _disposed;

    public ChunkControl(PointInt2 canvasPos)
    {
        RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.NearestNeighbor);
        RenderOptions.SetEdgeMode(this, EdgeMode.Aliased);
        _buff = new WriteableBitmap(16, 16, 96, 96, s_format, palette: null);
        Stretch = Stretch.UniformToFill;
        Source = _buff;
        _canvasPos = canvasPos;
    }

    public PointInt2 CanvasPos => _canvasPos;

    public void SetRandomImage()
    {
        try
        {
            _buff.Lock();
            Random random = new();
            for (int x = 0; x < Section.BlockCount; x++)
            {
                for (int z = 0; z < Section.BlockCount; z++)
                {
                    PointInt2 blockPixelPos = new(x, z);
                    Color color;
                    if (CanvasPos == PointInt2.Zero)
                    {
                        int col = random.Next(150, 250);
                        color = Color.FromArgb(
                            255,
                            col,
                            col,
                            col
                        );

                    }
                    else if ((_canvasPos.X + _canvasPos.Y) % 2 == 0)
                        color = Color.FromArgb(
                            255,
                            random.Next(0, 100),
                            random.Next(100, 200),
                            random.Next(150, 250)
                        );
                    else
                        color = Color.FromArgb(
                            255,
                            random.Next(150, 250),
                            random.Next(0, 100),
                            random.Next(0, 100)
                        );
                    SetBlockColor(blockPixelPos, color);
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

    #region Disposable Pattern

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
