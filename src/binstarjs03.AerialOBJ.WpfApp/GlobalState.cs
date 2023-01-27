using System;

using binstarjs03.AerialOBJ.WpfApp.Models;

using CommunityToolkit.Mvvm.ComponentModel;

namespace binstarjs03.AerialOBJ.WpfApp;

[ObservableObject]
public partial class GlobalState
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasSavegameLoaded))]
    private SavegameLoadInfo? _savegameLoadInfo = null;

    public GlobalState(DateTime launchTime, string version, string[]? arguments, ConstantPath path, SettingState setting)
    {
        LaunchTime = launchTime;
        Version = version;
        Arguments = arguments;
        Path = path;
        Setting = setting;
    }

    public event Action<SavegameLoadState>? SavegameLoadInfoChanged;

    public static string AppName => "AerialOBJ";
    public string Version { get; }
    public DateTime LaunchTime { get; }
    public ConstantPath Path { get; }
    public string[]? Arguments { get; }
    public bool HasSavegameLoaded => SavegameLoadInfo is not null;
    public bool IsDebugEnabled => Arguments is not null && Array.Exists(Arguments, arg => arg.ToLower() == "debug");
    public SettingState Setting { get; }

    partial void OnSavegameLoadInfoChanged(SavegameLoadInfo? value)
    {
        SavegameLoadState state;
        if (value is null)
            state = SavegameLoadState.Closed;
        else
            state = SavegameLoadState.Opened;
        SavegameLoadInfoChanged?.Invoke(state);
    }
}
