using System;
using System.Collections.Generic;
using System.Windows.Input;

using CommunityToolkit.Mvvm.Input;

namespace binstarjs03.AerialOBJ.WpfApp.Services.Input;
public partial class KeyHandler : IKeyHandler
{
    private readonly Dictionary<Key, Action> _keyDownHandler = new();
    private readonly Dictionary<Key, Action> _keyUpHandler = new();

    public void RegisterKeyDownHandler(Key key, Action method)
    {
        _keyDownHandler.Add(key, method);
    }

    public void RegisterKeyUpHandler(Key key, Action method)
    {
        _keyUpHandler.Add(key, method);
    }


    [RelayCommand]
    private void OnKeyDown(KeyEventArgs e)
    {
        Key downKey = e.Key;
        foreach ((Key key, Action handler) in _keyDownHandler)
        {
            if (downKey != key)
                continue;
            handler.Invoke();
            e.Handled = true;
            break;
        }
    }

    [RelayCommand]
    private void OnKeyUp(KeyEventArgs e)
    {
        Key upKey = e.Key;
        foreach ((Key key, Action handler) in _keyUpHandler)
        {
            if (upKey != key)
                continue;
            handler.Invoke();
            e.Handled = true;
            break;
        }
    }
}
