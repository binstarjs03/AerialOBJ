using binstarjs03.AerialOBJ.Core.Primitives;

namespace binstarjs03.AerialOBJ.WpfApp.Components;
public interface IMutableImage
{
    public Size<int> Size { get; }
    Color this[int x, int y] { get; set; }
    void Redraw();
}
