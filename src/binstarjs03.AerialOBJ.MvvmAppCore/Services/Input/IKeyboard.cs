using System;

using CommunityToolkit.Mvvm.Input;

namespace binstarjs03.AerialOBJ.MvvmAppCore.Services.Input;
public interface IKeyboard
{
    IRelayCommand<object?> KeyDownCommand { get; }

    void RegisterKeyDownHandler(object key, Action handler);
}
