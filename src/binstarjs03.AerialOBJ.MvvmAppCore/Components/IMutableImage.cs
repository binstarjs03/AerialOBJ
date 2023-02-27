using binstarjs03.AerialOBJ.Imaging;

namespace binstarjs03.AerialOBJ.MvvmAppCore.Components;

// we want an IImage that has the ability to redraw itself since it uses
// WriteableBitmap, which should add full dirtyrect when invoked
public interface IMutableImage : IImage
{
    void Redraw();
}
