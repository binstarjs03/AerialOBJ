using System;

namespace binstarjs03.AerialOBJ.WpfAppNew2.Components;
public interface IShowMessageBox
{
    event Action<string>? ShowMessageBoxRequested;
}
