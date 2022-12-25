using System;

namespace binstarjs03.AerialOBJ.WpfAppNew2.Components;
public class GlobalState
{
    private bool _isDebugLogViewVisible = false;
    private SavegameLoadInfo? _savegameLoadInfo = null;
    private bool _isViewportInfoVisible = false;
    private bool _isViewportChunkGridVisible = false;

    public GlobalState(DateTime launchTime)
    {
        LaunchTime = launchTime;
    }

    public static string AppName => "AerialOBJ";
    public string AppVersion { get; } = "InDev";
    public DateTime LaunchTime { get; }

    public event Action<string>? PropertyChanged;
    public event Action<SavegameLoadState>? SavegameLoadChanged;

    public bool IsDebugLogWindowVisible
    {
        get => _isDebugLogViewVisible;
        set
        {
            if (value == _isDebugLogViewVisible)
                return;
            _isDebugLogViewVisible = value;
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
            Reinitialize(loadState);
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
            OnPropertyChanged();
        }
    }

    public bool IsViewportChunkGridVisible
    {
        get => _isViewportChunkGridVisible;
        set
        {
            if (value == _isViewportChunkGridVisible)
                return;
            _isViewportChunkGridVisible = value;
            OnPropertyChanged();
        }
    }

    private void OnPropertyChanged()
    {
        PropertyChanged?.Invoke(nameof(GlobalState));
    }

    private void Reinitialize(SavegameLoadState state)
    {
        if (state == SavegameLoadState.Closed)
        {
            IsViewportInfoVisible = false;
            IsViewportChunkGridVisible = false;
        }
#if DEBUG
        else
        {
            IsViewportInfoVisible = true;
            IsViewportChunkGridVisible = true;
        }
#endif
    }
}
