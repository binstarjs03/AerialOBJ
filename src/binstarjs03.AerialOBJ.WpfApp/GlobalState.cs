using System;

using binstarjs03.AerialOBJ.WpfApp.Components;

namespace binstarjs03.AerialOBJ.WpfApp;
public class GlobalState
{
    private SavegameLoadInfo? _savegameLoadInfo = null;

    public GlobalState(DateTime launchTime, string version)
    {
        LaunchTime = launchTime;
        Version = version;
    }

    public event Action<string>? PropertyChanged;
    public event Action<SavegameLoadState>? SavegameLoadChanged;

    public static string AppName => "AerialOBJ";
    public string Version { get; }
    public DateTime LaunchTime { get; }
    public bool HasSavegameLoaded => SavegameLoadInfo is not null;

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
            if (loadState == SavegameLoadState.Closed)
                GC.Collect();
            OnPropertyChanged();
        }
    }

    private void OnPropertyChanged() => PropertyChanged?.Invoke(nameof(GlobalState));
}
