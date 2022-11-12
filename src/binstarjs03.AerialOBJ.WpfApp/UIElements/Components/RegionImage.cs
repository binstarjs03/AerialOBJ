/*
Copyright (c) 2022, Bintang Jakasurya
All rights reserved. 

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

using binstarjs03.AerialOBJ.Core.MinecraftWorld;

namespace binstarjs03.AerialOBJ.WpfApp.UIElements.Components;

/// <summary>
/// Wrapper around <see cref="System.Windows.Controls.Image"/> and
/// <see cref="System.Windows.Media.Imaging.WriteableBitmap"/> combo.
/// This class allows you to set pixel color from any thread but updating the
/// change to make it visible to the screen still must be done from UI Thread
/// </summary>
public class RegionImage
{
    private const int s_blockCount = Region.ChunkCount * Section.BlockCount;
    private const int s_pixelLength = s_blockCount;
    private static readonly PixelFormat s_pixelFormat = PixelFormats.Bgra32;

    private readonly Image _image;
    private readonly WriteableBitmap _writeableBitmap;
    private readonly IntPtr _backBufferPtr;

    public Image Image => _image;
    public WriteableBitmap WriteableBitmap => _writeableBitmap;

    /// <summary>
    /// Instantiation must be done from UI thread else <see cref="InvalidOperationException"/>
    /// will be thrown
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    public RegionImage()
    {
        if (!App.CheckAccess())
            throw new InvalidOperationException("Instantiation must be done from UI thread");
        _writeableBitmap = new WriteableBitmap(s_pixelLength, s_pixelLength,
                                               96, 96,
                                               s_pixelFormat,
                                               null);
        _backBufferPtr = _writeableBitmap.BackBuffer;
        _image = new Image
        {
            Stretch = Stretch.Uniform,
            Source = _writeableBitmap
        };
        RenderOptions.SetBitmapScalingMode(_image, BitmapScalingMode.NearestNeighbor);
        RenderOptions.SetEdgeMode(_image, EdgeMode.Aliased);
    }

    /// <summary>
    /// Makes change from <see cref="SetPixel"/> visible to the screen, 
    /// can be invoked from any thread. Will invoke dispatcher if the calling
    /// thread isn't UI Thread.
    /// </summary>
    public void Redraw()
    {
        if (App.CheckAccess())
            method();
        else
            App.InvokeDispatcher(method,
                                 DispatcherPriority.Render,
                                 DispatcherSynchronization.Synchronous);
        void method()
        {
            _writeableBitmap.Lock();
            AddFullDirtyRect();
            _writeableBitmap.Unlock();
        }
    }

    private void AddFullDirtyRect()
    {
        Int32Rect dirtyRect = new(x: 0, y: 0,
                                  width: s_pixelLength,
                                  height: s_pixelLength);
        _writeableBitmap.AddDirtyRect(dirtyRect);
    }

    /// <summary>
    /// Set pixel color at XY, can be invoked from any thread
    /// </summary>
    public void SetPixel(int x, int y, Color color)
    {
        /* Data format for Bgra32:
         * Bgra32 stores each pixel with 4 channels:
         * B: Blue color
         * G: Green color
         * R: Red color
         * A: Alpha intensity
         * 
         * The way Bgra32 organizes the data is self-explanatory:
         * the first byte is the blue color, second is green, 
         * third is red, and the last, fourth byte is the alpha.
         * 
         * Lets say we have an image that has 2*2 pixels. The first pixel (0,0) is stored at 
         * exactly at index 0, and since a single pixel has 4 bytes for each 4 channels, 
         * the first pixel stored at index 0 to 3. The second pixel (1,0) is stored after 
         * the first pixel, and that is at index 4 to 7, then the third pixel (0,1) is
         * stored after the second pixel and so on. So the ordering for the byte array is
         * X pixels goes arranged first, then the next Y pixels
         * 
         * Now, how do we know the length of the bytes array for Bgra32? its simple. First, 
         * you need to calculate total pixels in your image. In our image, the Region Image, 
         * a single pixel represent single block, and it has 512 * 512 blocks (in 2D)
         * so our Region Image has 512 * 512 pixels. Since Bgra32 store 4 bytes per pixel,
         * that means our Region Image byte array length is 512 * 512 * 4 = 1.048.576,
         * yes you read that right... one million bytes in a single array.
         */

        int bitsPerByte = 8;
        int bytesPerPixel = s_pixelFormat.BitsPerPixel / bitsPerByte;
        int xOffset = x * bytesPerPixel;
        int yOffset = y * bytesPerPixel * s_pixelLength;
        int offset = xOffset + yOffset;

        unsafe
        {
            byte* backBuffData = (byte*)_backBufferPtr.ToPointer();
            backBuffData[offset + 0] = color.B;
            backBuffData[offset + 1] = color.G;
            backBuffData[offset + 2] = color.R;
            backBuffData[offset + 3] = color.A;
        }
    }
}
