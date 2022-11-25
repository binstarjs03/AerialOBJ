using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

using binstarjs03.AerialOBJ.Core.MinecraftWorld;
using binstarjs03.AerialOBJ.Core.Primitives;

namespace binstarjs03.AerialOBJ.WpfAppNew.Models;

public class RegionModel
{
    private const int s_pixelLength = Region.BlockCount;
    private static readonly PixelFormat s_pixelFormat = PixelFormats.Bgra32;
    private readonly WriteableBitmap _bitmap;
    private readonly IntPtr _backBuff;

    public Region Region { get; }
    public HashSet<Point2Z<int>> GeneratedChunks { get; }
    public Point2Z<int> RegionCoords { get; }
    public Image Image { get; }
    public bool NeedRedrawBitmap { get; set; } = true;

    public RegionModel(Region region)
    {
        Region = region;
        GeneratedChunks = region.GetGeneratedChunksAsCoordsRelSet();
        RegionCoords = region.RegionCoords;
        _bitmap = new WriteableBitmap(s_pixelLength, s_pixelLength,
                                      96, 96,
                                      s_pixelFormat, null);
        _backBuff = _bitmap.BackBuffer;
        Image = new Image
        {
            Stretch = Stretch.Uniform,
            Source = _bitmap
        };
        RenderOptions.SetBitmapScalingMode(Image, BitmapScalingMode.NearestNeighbor);
        RenderOptions.SetEdgeMode(Image, EdgeMode.Aliased);
    }

    public void SetPixel(int x, int y, Color color)
    {
        int bitsPerByte = 8;
        int bytesPerPixel = s_pixelFormat.BitsPerPixel / bitsPerByte;
        int xOffset = x * bytesPerPixel;
        int yOffset = y * bytesPerPixel * s_pixelLength;
        int offset = xOffset + yOffset;

        unsafe
        {
            byte* backBuffData = (byte*)_backBuff.ToPointer();
            backBuffData[offset + 0] = color.B;
            backBuffData[offset + 1] = color.G;
            backBuffData[offset + 2] = color.R;
            backBuffData[offset + 3] = color.A;
        }
    }

    public void SetRandomImage()
    {
        Random random = new();
        Color color = Color.FromRgb((byte)random.Next(0, 255),
                                    (byte)random.Next(0, 255),
                                    (byte)random.Next(0, 255));
        for (int x = 0; x < s_pixelLength; x++)
            for (int y = 0; y < s_pixelLength; y++)
                SetPixel(x, y, color);
    }

    public void RedrawImage(Point2<int> imageScreenPos, float size)
    {
        if (App.CheckAccess())
            method();
        else
            App.InvokeDispatcher(method, DispatcherPriority.Render, DispatcherSynchronization.Synchronous);
        void method()
        {
            if (NeedRedrawBitmap)
            {
                RedrawBitmap();
                NeedRedrawBitmap = false;
            }
            UpdateImagePosition(imageScreenPos);
            UpdateImageSize(size);
        }
    }

    private void RedrawBitmap()
    {
        if (App.CheckAccess())
            method();
        else
            App.InvokeDispatcher(method,
                                 DispatcherPriority.Render,
                                 DispatcherSynchronization.Synchronous);
        void method()
        {
            _bitmap.Lock();
            Int32Rect dirtyRect = new(x: 0, y: 0,
                                  width: s_pixelLength,
                                  height: s_pixelLength);
            _bitmap.AddDirtyRect(dirtyRect);
            _bitmap.Unlock();
        }
    }

    private void UpdateImagePosition(Point2<int> screenPos)
    {
        Canvas.SetLeft(Image, screenPos.X);
        Canvas.SetTop(Image, screenPos.Y);
    }

    private void UpdateImageSize(float size)
    {
        Image.Width = size;
    }
}
