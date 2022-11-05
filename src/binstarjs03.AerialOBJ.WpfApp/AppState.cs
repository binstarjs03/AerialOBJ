using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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
