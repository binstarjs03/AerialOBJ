using binstarjs03.AerialOBJ.Core.Primitives;

namespace binstarjs03.AerialOBJ.MvvmAppCore.Services;
public interface ISizeConverter
{
    Size<int> Convert(object value);
}
