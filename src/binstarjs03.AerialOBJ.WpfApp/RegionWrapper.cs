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
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

using binstarjs03.AerialOBJ.Core.CoordinateSystem;
using binstarjs03.AerialOBJ.Core.MinecraftWorld;
using binstarjs03.AerialOBJ.WpfApp.Converters;
using binstarjs03.AerialOBJ.WpfApp.UIElements.Components;
using binstarjs03.AerialOBJ.WpfApp.UIElements.Controls;

using Color = System.Windows.Media.Color;
using Region = binstarjs03.AerialOBJ.Core.MinecraftWorld.Region;

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
    public bool NeedRedraw { get; set; }

    public RegionWrapper(Region region, ViewportControlVM viewport)
    {
        _region = region;
        _regionImage = App.InvokeDispatcherSynchronous(
            () => new RegionImage(),
            DispatcherPriority.Background);
        _viewport = viewport;
        _regionCoords = region.RegionCoords;
        _generatedChunks = region.GetGeneratedChunksAsCoordsRelSet();
    }

    public Chunk GetChunk(Coords2 chunkCoords, bool relative)
    {
        return _region.GetChunk(chunkCoords, relative);
    }

    public bool HasChunkGenerated(Coords2 chunkCoordsRel)
    {
        return _generatedChunks.Contains(chunkCoordsRel);
    }

    // This method was added for debugging purpose whether Region is loaded or not
    [EditorBrowsable(EditorBrowsableState.Never)]
    public void SetRandomImage()
    {
        Color color;
        if (RegionCoords == Coords2.Zero)
            color = Color.FromArgb(255, 255, 255, 255);
        else
        {
            Random random = new();
            Span<byte> buff = stackalloc byte[3];
            random.NextBytes(buff);
            color = Color.FromArgb(255, buff[0], buff[1], buff[2]);
        }
        for (int x = 0; x < Region.BlockCount; x++)
            for (int z = 0; z < Region.BlockCount; z++)
                _regionImage.SetPixel(x, z, color);
        NeedRedraw = true;
    }

    public void BlitChunkImage(Coords2 chunkCoordsRel, ChunkHighestBlockInfo highestBlock)
    {
        for (int x = 0; x < Section.BlockCount; x++)
            for (int z = 0; z < Section.BlockCount; z++)
            {
                Color color = BlockToColor.Convert(highestBlock.Names[x, z]);
                _regionImage.SetPixel(chunkCoordsRel.X * Section.BlockCount + x,
                                      chunkCoordsRel.Z * Section.BlockCount + z,
                                      color);
            }
        NeedRedraw = true;
    }

    public void EraseChunkImage(Coords2 chunkCoordsRel)
    {
        for (int x = 0; x < Section.BlockCount; x++)
            for (int z = 0; z < Section.BlockCount; z++)
            {
                _regionImage.SetPixel(chunkCoordsRel.X * Section.BlockCount + x,
                                      chunkCoordsRel.Z * Section.BlockCount + z,
                                      Colors.Transparent);
            }
        NeedRedraw = true;
    }

    public void RedrawImage()
    {
        if (App.CheckAccess())
            method();
        else
            App.InvokeDispatcher(method, DispatcherPriority.Render, DispatcherSynchronization.Synchronous);
        void method()
        {
            if (NeedRedraw)
            {
                _regionImage.Redraw();
                NeedRedraw = false;
            }
            UpdateImagePosition();
            UpdateImageSize();
        }
    }

    private void UpdateImagePosition()
    {
        // if region position is 2, then the world position for it is
        // block count in region * position = 512 * 2 = 1024
        PointF2 regionWorldPos = ((PointF2)_regionCoords) * Region.BlockCount;
        PointF2 screenPos = _viewport.ConvertWorldPositionToScreenPosition(regionWorldPos);
        Canvas.SetLeft(_regionImage.Image, screenPos.X);
        Canvas.SetTop(_regionImage.Image, screenPos.Y);
    }

    private void UpdateImageSize()
    {
        _regionImage.Image.Width = _viewport.PixelPerRegion;
    }
}
