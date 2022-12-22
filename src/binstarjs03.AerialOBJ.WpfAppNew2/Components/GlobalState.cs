using System;

namespace binstarjs03.AerialOBJ.WpfAppNew2.Components;
public class GlobalState
{
    private bool _isDebugLogViewVisible = false;
    private SavegameLoadInfo? _savegameLoadInfo = null;
    private bool _isViewportInfoVisible = false;

    public GlobalState(DateTime launchTime)
    {
        LaunchTime = launchTime;
    }

    public static string AppName => "AerialOBJ";
    public string AppVersion { get; } = "InDev";
    public DateTime LaunchTime { get; }

    public event Action<string>? PropertyChanged;
    public event Action<bool>? DebugLogViewVisibilityChanged;
    public event Action<bool>? ViewportInfoVisibilityChanged;
    public event Action<SavegameLoadState>? SavegameLoadChanged;

    public bool IsDebugLogWindowVisible
    {
        get => _isDebugLogViewVisible;
        set
        {
            if (value == _isDebugLogViewVisible)
                return;
            _isDebugLogViewVisible = value;
            DebugLogViewVisibilityChanged?.Invoke(value);
            OnPropertyChanged();
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
            Reinitialize();
            OnPropertyChanged();
        }
    }

    public bool HasSavegameLoaded => SavegameLoadInfo is not null;

    public bool IsViewportInfoVisible
    {
        get => _isViewportInfoVisible;
        set
        {
            if (value == _isViewportInfoVisible)
                return;
            _isViewportInfoVisible = value;
            ViewportInfoVisibilityChanged?.Invoke(value);
            OnPropertyChanged();
        }
    }

    private void OnPropertyChanged()
    {
        PropertyChanged?.Invoke(nameof(GlobalState));
    }

    private void Reinitialize()
    {
#if DEBUG
        IsViewportInfoVisible = true;
#elif RELEASE
        IsViewportInfoVisible = false;
#endif
    }
}
