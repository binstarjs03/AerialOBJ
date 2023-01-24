using binstarjs03.AerialOBJ.Core.Primitives;

namespace binstarjs03.AerialOBJ.Imaging;
public interface IImage
{
    Size<int> Size { get; }
    Color this[int x, int y] { get; set; }
}
