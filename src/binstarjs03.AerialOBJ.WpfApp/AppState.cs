/*
Copyright (c) 2022, Bintang Jakasurya
All rights reserved. 

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/


using System;

namespace binstarjs03.AerialOBJ.WpfApp;

/// <summary>
/// Provides Application-scope states/properties
/// </summary>
public class AppState
{
    public event Action<bool>? DebugLogWindowVisibleChanged;
    public event SavegameLoadStateHandler? SavegameLoadChanged;

    private readonly DateTime _lauchTime;
    private bool _debugLogWindowVisible = false;
    private SavegameLoadInfo? _savegameLoadInfo = null;

    public AppState()
    {
        _lauchTime = DateTime.Now;
    }

    public static string AppName => "AerialOBJ";
    public DateTime LaunchTime => _lauchTime;

    public bool DebugLogWindowVisible
    {
        get => _debugLogWindowVisible;
        set
        {
            _debugLogWindowVisible = value;
            DebugLogWindowVisibleChanged?.Invoke(value);
        }
    }

    public SavegameLoadInfo? SavegameLoadInfo
    {
        get => _savegameLoadInfo;
        set
        {
            _savegameLoadInfo = value;
            SavegameLoadState state;
            if (value is null)
                state = SavegameLoadState.Closed;
            else
                state = SavegameLoadState.Opened;
            SavegameLoadChanged?.Invoke(state);
        }
    }

    public bool HasSavegameLoaded => SavegameLoadInfo is not null;
}
