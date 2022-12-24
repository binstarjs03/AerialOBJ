using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace binstarjs03.AerialOBJ.WpfAppNew2.Components;
using binstarjs03.AerialOBJ.Core.Primitives;

public class MutableImage : Image, IMutableImage
{
    private readonly PixelFormat s_pixelFormat = PixelFormats.Bgra32;
    private readonly WriteableBitmap _writeableBitmap;
    private readonly nint _backBuff;

    public Size<int> Size { get; }

    public MutableImage(Size<int> imageSize)
    {
        Size = imageSize;
        _writeableBitmap = new WriteableBitmap(imageSize.Width, imageSize.Height,
                                               96, 96,
                                               s_pixelFormat, null);
        _backBuff = _writeableBitmap.BackBuffer;
        Source = _writeableBitmap;
    }

    public void Redraw()
    {
        if (CheckAccess())
            AddFullDirtyRect();
        else
            Dispatcher.BeginInvoke(AddFullDirtyRect, DispatcherPriority.Render, null);
    }

    private void AddFullDirtyRect()
    {
        _writeableBitmap.Lock();
        _writeableBitmap.AddDirtyRect(new Int32Rect(0, 0, Size.Width, Size.Height));
        _writeableBitmap.Unlock();
    }

    // TODO add safety to throw an exception if pixelPos exceed image size
    public void SetPixel(Point2<int> pixelPos, Color color)
    {
        int bitsPerByte = 8;
        int bytesPerPixel = s_pixelFormat.BitsPerPixel / bitsPerByte;
        int xOffset = pixelPos.X * bytesPerPixel;
        int yOffset = pixelPos.Y * bytesPerPixel * Size.Width;
        int offset = xOffset + yOffset;

        unsafe
        {
            byte* backBuffData = (byte*)_backBuff.ToPointer();
            backBuffData[offset + 0] = color.Blue;
            backBuffData[offset + 1] = color.Green;
            backBuffData[offset + 2] = color.Red;
            backBuffData[offset + 3] = color.Alpha;
        }
    }
}
