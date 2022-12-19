using System;

namespace binstarjs03.AerialOBJ.WpfAppNew2.Components;
public class GlobalState
{
    public event Action<bool>? DebugLogViewVisibilityChanged;

    private bool s_isDebugLogViewVisible = false;

    public GlobalState(DateTime launchTime)
    {
        LaunchTime = launchTime;
    }
    public static string AppName => "AerialOBJ";
    public string AppVersion { get; } = "InDev";
    public DateTime LaunchTime { get; }

    public bool IsDebugLogWindowVisible
    {
        get => s_isDebugLogViewVisible;
        set
        {
            if (value != s_isDebugLogViewVisible)
            {
                s_isDebugLogViewVisible = value;
                DebugLogViewVisibilityChanged?.Invoke(value);
            }
        }
    }
}
