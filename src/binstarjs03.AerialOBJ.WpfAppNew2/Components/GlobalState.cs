using System;

namespace binstarjs03.AerialOBJ.WpfAppNew2.Components;
public class GlobalState
{
    private bool _isDebugLogViewVisible = false;
    private SavegameLoadInfo? _savegameLoadInfo = null;

    public GlobalState(DateTime launchTime)
    {
        LaunchTime = launchTime;
    }

    public static string AppName => "AerialOBJ";
    public string AppVersion { get; } = "InDev";
    public DateTime LaunchTime { get; }

    public event Action<bool>? DebugLogViewVisibilityChanged;
    public event Action<SavegameLoadState>? SavegameLoadChanged;

    public bool IsDebugLogWindowVisible
    {
        get => _isDebugLogViewVisible;
        set
        {
            if (value != _isDebugLogViewVisible)
            {
                _isDebugLogViewVisible = value;
                DebugLogViewVisibilityChanged?.Invoke(value);
            }
        }
    }

    public SavegameLoadInfo? SavegameLoadInfo
    {
        get => _savegameLoadInfo;
        set
        {
            if (value == _savegameLoadInfo)
                return;
            _savegameLoadInfo = value;
            SavegameLoadState loadState = value is null ? 
                SavegameLoadState.Closed : SavegameLoadState.Opened;
            SavegameLoadChanged?.Invoke(loadState);
        }
    }

    public bool HasSavegameLoaded => SavegameLoadInfo is not null;
}
