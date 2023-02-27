using System;
using System.Collections.Generic;
using System.Windows.Input;

using binstarjs03.AerialOBJ.MvvmAppCore.Services.Input;

using CommunityToolkit.Mvvm.Input;

namespace binstarjs03.AerialOBJ.WpfApp.Services.Input;
public partial class Keyboard : IKeyboard
{
    private readonly Dictionary<Key, Action> _keyDownHandlers = new();

    public void RegisterKeyDownHandler(object key, Action handler)
    {
        _keyDownHandlers.Add((Key)key, handler);
    }

    [RelayCommand]
    private void OnKeyDown(object e)
    {
        var arg = CheckAndCast<KeyEventArgs>(e);
        Key downKey = arg.Key;
        foreach ((Key key, Action handler) in _keyDownHandlers)
        {
            if (downKey != key)
                continue;
            handler.Invoke();
            arg.Handled = true;
            break;
        }
    }

    private T CheckAndCast<T>(object e) where T : class
    {
        if (e is not T arg)
            throw new ArgumentException($"Argument must be typeof {nameof(T)}", nameof(e));
        return arg;
    }
}
