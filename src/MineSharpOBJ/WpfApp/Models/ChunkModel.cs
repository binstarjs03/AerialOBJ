using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using Color = System.Drawing.Color;
using Image = System.Windows.Controls.Image;

using Point = binstarjs03.MineSharpOBJ.Core.Utils.Point;
using Section = binstarjs03.MineSharpOBJ.Core.RegionMc.Section;

namespace binstarjs03.MineSharpOBJ.WpfApp.Models;

public class ChunkModel : Image {
    private static readonly PixelFormat s_format = PixelFormats.Bgra32;
    private static readonly int s_bitsPerByte = 8;
    private static readonly int s_bytesPerPixel = s_format.BitsPerPixel / s_bitsPerByte;

    private readonly Point _canvasPos;
    private readonly WriteableBitmap _buff;

    static ChunkModel() {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(ChunkModel), new FrameworkPropertyMetadata(typeof(ChunkModel)));
    }

    public ChunkModel(Point coordsAbs) {
        RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.NearestNeighbor);
        RenderOptions.SetEdgeMode(this, System.Windows.Media.EdgeMode.Aliased);
        _buff = new WriteableBitmap(16, 16, 96, 96, s_format, palette: null);
        Stretch = Stretch.UniformToFill;
        Source = _buff;
        _canvasPos = coordsAbs;
    }

    public Point CanvasPos => _canvasPos;

    public void SetRandomImage(bool red) {
        try {
            _buff.Lock();
            Random random = new();
            for (int x = 0; x < Section.BlockCount; x++) {
                for (int z = 0; z < Section.BlockCount; z++) {
                    Point blockPixelPos = new(x, z);
                    Color color;
                    if (_canvasPos == Point.Origin) {
                        int col = random.Next(150, 250);
                        color = Color.FromArgb(
                            255,
                            col,
                            col,
                            col
                        );

                    }
                    else if (red)
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
        finally {
            _buff.Unlock();
        }
    }

    private void SetBlockColor(Point blockPixelPos, Color color) {
        //if (pixelPos.X > 15)
        int xOffset = blockPixelPos.X * s_bytesPerPixel;
        int yOffset = blockPixelPos.Y * s_bytesPerPixel * _buff.PixelWidth;
        int offset = xOffset + yOffset;
        IntPtr backBuffPtr = _buff.BackBuffer;
        unsafe {
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
}
