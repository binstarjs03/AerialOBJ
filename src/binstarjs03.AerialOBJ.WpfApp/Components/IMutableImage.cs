using binstarjs03.AerialOBJ.Core.Primitives;

namespace binstarjs03.AerialOBJ.WpfApp.Components;
public delegate void ForeachPixelDelegate(IMutableImage self, int x, int y);
public interface IMutableImage
{
    Size<int> Size { get; }
    Color this[int x, int y] { get; set; }
    void Redraw();
    void ForeachPixels(ForeachPixelDelegate callback);
}
