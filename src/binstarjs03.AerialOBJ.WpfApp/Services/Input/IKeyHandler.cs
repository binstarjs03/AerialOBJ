using System;
using System.Windows.Input;

namespace binstarjs03.AerialOBJ.WpfApp.Services.Input;
public interface IKeyHandler
{
    void RegisterKeyDownHandler(Key key, Action method);
    void RegisterKeyUpHandler(Key key, Action method);
}
