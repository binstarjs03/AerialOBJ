using binstarjs03.AerialOBJ.Core.Primitives;
using binstarjs03.AerialOBJ.WpfAppNew2.Components;

namespace binstarjs03.AerialOBJ.WpfAppNew2.Factories;
public class MutableImageFactory : IMutableImageFactory
{
    public IMutableImage Create(Size<int> size)
    {
        if (App.Current.CheckAccess())
            return new MutableImage(size);
        else
            return App.Current.Dispatcher.Invoke(() => new MutableImage(size));
    }
}
