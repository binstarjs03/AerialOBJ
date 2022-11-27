using System;
using System.Collections.Generic;

using binstarjs03.AerialOBJ.Core.MinecraftWorld;
using binstarjs03.AerialOBJ.Core.Primitives;

namespace binstarjs03.AerialOBJ.Core.Visualization;
public interface IRegionModel
{
    Region Region { get; }
    HashSet<Point2Z<int>> GeneratedChunks { get; }
    Point2Z<int> RegionCoords { get; }
    IImage RegionImage { get; }
    bool NeedRedrawBitmap { get; set; }

    void SetRandomImage();
    void SetPixel(int x, int y, Span<byte> colorArgb);
    void RedrawImage(Point2<int> imageScreenPos, float size);
}
