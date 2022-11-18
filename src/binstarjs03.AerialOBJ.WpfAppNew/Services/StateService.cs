using System;

using binstarjs03.AerialOBJ.WpfAppNew.Components;
using binstarjs03.AerialOBJ.WpfAppNew.Models;

namespace binstarjs03.AerialOBJ.WpfAppNew.Services;
public static class StateService
{
    public static event Action<bool>? DebugLogWindowVisibleChanged;
    public static event SavegameLoadStateHandler? SavegameLoadChanged;

    private static bool s_debugLogWindowVisible = false;
    private static SavegameLoadInfo? s_savegameLoadInfo = null;

    public static DateTime LaunchTime { get; } = DateTime.Now;
    public static string AppName => "AerialOBJ";
    public static string AppVersion { get; } = "InDev";
    public static bool DebugLogWindowVisible
    {
        get => s_debugLogWindowVisible;
        set
        {
            if (value != s_debugLogWindowVisible)
            {
                s_debugLogWindowVisible = value;
                DebugLogWindowVisibleChanged?.Invoke(value);
            }
        }
    }
    public static SavegameLoadInfo? SavegameLoadInfo
    {
        get => s_savegameLoadInfo;
        set
        {
            if (value != s_savegameLoadInfo)
            {
                s_savegameLoadInfo = value;
                SavegameLoadState state;
                if (value is null)
                    state = SavegameLoadState.Closed;
                else
                    state = SavegameLoadState.Opened;
                SavegameLoadChanged?.Invoke(state);
            }
        }
    }
    public static bool HasSavegameLoaded => SavegameLoadInfo is not null;
}
