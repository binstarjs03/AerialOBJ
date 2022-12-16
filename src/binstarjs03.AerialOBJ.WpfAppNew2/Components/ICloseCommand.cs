using System;

using CommunityToolkit.Mvvm.Input;

namespace binstarjs03.AerialOBJ.WpfAppNew2.Components;
public interface ICloseCommand
{
    event Action CloseRequested;
    IRelayCommand CloseCommand { get; }
}
