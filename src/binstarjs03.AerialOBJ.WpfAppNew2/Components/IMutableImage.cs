using binstarjs03.AerialOBJ.Core.Primitives;

namespace binstarjs03.AerialOBJ.WpfAppNew2.Components;
public interface IMutableImage
{
    public Size<int> Size { get; }
    void SetPixel(Point2<int> pos, Color color);
    void Redraw();
}
