using System;

using binstarjs03.AerialOBJ.WpfAppNew2.Components;

namespace binstarjs03.AerialOBJ.WpfAppNew2.Services;
public class LogService : ILogService
{
    private string s_logContent = "";
    private GlobalState _globalState;

    public string LogContent => s_logContent;

    public event Action? Logging;

    public LogService(GlobalState globalState)
    {
        _globalState = globalState;
    }

    public void Clear()
    {
        s_logContent = "";
    }

    public void Log(string message, bool useSeparator = false)
    {
        s_logContent += $"{message}{Environment.NewLine}";
        if (useSeparator)
            s_logContent += Environment.NewLine;
        Logging?.Invoke();
    }

    public void LogEmphasis(string message, Emphasis emphasis, bool useSeparator = false)
    {
        Log($"[{emphasis.ToString().ToUpper()}] {message}", useSeparator);
    }

    public void LogRuntimeInfo()
    {
        Log($"Launch time: {_globalState.LaunchTime}");
        Log($"{GlobalState.AppName} Version: ...");
        Log("Commit Hash: ...", useSeparator: true);

        Log($"Host OS: {Environment.OSVersion}");
        Log($"CPU cores count: {Environment.ProcessorCount}");
        Log($".NET Runtime Version: {Environment.Version}", useSeparator: true);
    }
}
