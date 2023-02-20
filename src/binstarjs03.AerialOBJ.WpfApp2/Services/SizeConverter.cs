using System.Windows;

using binstarjs03.AerialOBJ.Core;
using binstarjs03.AerialOBJ.Core.Primitives;
using binstarjs03.AerialOBJ.MvvmAppCore.Services;

namespace binstarjs03.AerialOBJ.WpfApp.Services;
public class SizeConverter : ISizeConverter
{
    public Size<int> Convert(object value)
    {
        if (value is SizeChangedEventArgs sizeChangedEvent)
        {
            var size = sizeChangedEvent.NewSize;
            return new Size<int>(size.Width.Floor(), size.Height.Floor());
        }
        else if (value is Size wpfSize)
            return new Size<int>(wpfSize.Width.Floor(), wpfSize.Height.Floor());
        throw new System.NotImplementedException();
    }
}
