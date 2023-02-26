using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace binstarjs03.AerialOBJ.WpfApp.Components;

using System;

using binstarjs03.AerialOBJ.Core.Primitives;
using binstarjs03.AerialOBJ.MvvmAppCore.Components;

public class MutableImage : Image, IMutableImage
{
    private readonly PixelFormat _pixelFormat = PixelFormats.Bgra32;
    private readonly WriteableBitmap _writeableBitmap;
    private readonly nint _backBuff;

    public MutableImage(Size<int> imageSize)
    {
        Size = imageSize;
        _writeableBitmap = new WriteableBitmap(imageSize.Width, imageSize.Height,
                                               96, 96,
                                               _pixelFormat, null);
        _backBuff = _writeableBitmap.BackBuffer;
        Source = _writeableBitmap;
    }

    public Size<int> Size { get; }

    public Color this[int x, int y]
    {
        get
        {
            int offset = ConvertPixelCoordsToStartOffset(x, y);
            unsafe
            {
                byte* backBuffData = (byte*)_backBuff.ToPointer();
                return new Color()
                {
                    Blue = backBuffData[offset + 0],
                    Green = backBuffData[offset + 1],
                    Red = backBuffData[offset + 2],
                    Alpha = backBuffData[offset + 3],
                };
            }
        }
        set
        {
            int offset = ConvertPixelCoordsToStartOffset(x, y);
            unsafe
            {
                byte* backBuffData = (byte*)_backBuff.ToPointer();
                backBuffData[offset + 0] = value.Blue;
                backBuffData[offset + 1] = value.Green;
                backBuffData[offset + 2] = value.Red;
                backBuffData[offset + 3] = value.Alpha;
            }
        }
    }

    public void Redraw()
    {
        if (CheckAccess())
            AddFullDirtyRect();
        else
            Dispatcher.InvokeAsync(AddFullDirtyRect, DispatcherPriority.Render);
    }

    private void AddFullDirtyRect()
    {
        _writeableBitmap.Lock();
        _writeableBitmap.AddDirtyRect(new Int32Rect(0, 0, Size.Width, Size.Height));
        _writeableBitmap.Unlock();
    }

    private int ConvertPixelCoordsToStartOffset(int x, int y)
    {
        if (x > Size.Width || y > Size.Height)
            throw new IndexOutOfRangeException("Pixel coordinate exceed image size");
        int bitsPerByte = 8;
        int bytesPerPixel = _pixelFormat.BitsPerPixel / bitsPerByte;
        int xOffset = x * bytesPerPixel;
        int yOffset = y * bytesPerPixel * Size.Width;
        int offset = xOffset + yOffset;
        return offset;
    }
}