using System;

using binstarjs03.AerialOBJ.WpfAppNew.Components;

namespace binstarjs03.AerialOBJ.WpfAppNew.Services;

public static class SharedStateService
{
    public static event Action<bool>? DebugLogWindowVisibilityChanged;
    public static event Action<bool>? ViewportSidePanelVisibilityChanged;
    public static event Action<bool>? ViewportSidePanelDebugInfoVisibilityChanged;
    public static event SavegameLoadStateHandler? SavegameLoadChanged;

    private static bool s_isDebugLogWindowVisible = false;
    private static bool s_isViewportSidePanelVisible = false;
    private static bool s_isViewportSidePanelDebugInfoVisible = false;
    private static SavegameLoadInfo? s_savegameLoadInfo = null;

    public static DateTime LaunchTime { get; } = DateTime.Now;
    public static string AppName => "AerialOBJ";
    public static string AppVersion { get; } = "InDev";

    public static bool IsDebugLogWindowVisible
    {
        get => s_isDebugLogWindowVisible;
        set
        {
            if (value != s_isDebugLogWindowVisible)
            {
                s_isDebugLogWindowVisible = value;
                DebugLogWindowVisibilityChanged?.Invoke(value);
            }
        }
    }
    public static bool IsViewportSidePanelVisible
    {
        get => s_isViewportSidePanelVisible;
        set
        {
            if (value != s_isViewportSidePanelVisible)
            {
                s_isViewportSidePanelVisible = value;
                ViewportSidePanelVisibilityChanged?.Invoke(value);
            }
        }
    }
    public static bool IsViewportSidePanelDebugInfoVisible
    {
        get => s_isViewportSidePanelDebugInfoVisible;
        set
        {
            if (value != s_isViewportSidePanelDebugInfoVisible)
            {
                s_isViewportSidePanelDebugInfoVisible = value;
                ViewportSidePanelDebugInfoVisibilityChanged?.Invoke(value);
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
