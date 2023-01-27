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

    public GlobalState(DateTime launchTime, string version, string currentPath, string definitionsPath, string[]? arguments, SettingState setting)
    {
        LaunchTime = launchTime;
        Version = version;
        CurrentPath = currentPath;
        DefinitionsPath = definitionsPath;
        Arguments = arguments;
        Setting = setting;
    }

    public event Action<SavegameLoadState>? SavegameLoadInfoChanged;

    public static string AppName => "AerialOBJ";
    public string Version { get; }
    public DateTime LaunchTime { get; }
    public string CurrentPath { get; } // current path to "AerialOBJ.exe"
    public string DefinitionsPath { get; }
    public string[]? Arguments { get; }
    public bool HasSavegameLoaded => SavegameLoadInfo is not null;
    public bool IsDebugEnabled => Arguments is not null && Array.Exists(Arguments, arg => arg.ToLower() == "debug");
    public SettingState Setting { get; set; }

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
