using System;

using binstarjs03.AerialOBJ.WpfApp.Components;

namespace binstarjs03.AerialOBJ.WpfApp.Services;
public class LogService : ILogService
{
    private string _logContent = "";
    private readonly GlobalState _globalState;

    public event LoggingEventHandler? Logging;

    public string LogContent => _logContent;

    public LogService(GlobalState globalState)
    {
        _globalState = globalState;
    }

    public void Clear()
    {
        _logContent = "";
    }

    public void Log(string message, bool useSeparator = false)
    {
        Log(message, LogStatus.Normal, useSeparator);
    }

    public void Log(string message, LogStatus status, bool useSeparator = false)
    {
        if (status != LogStatus.Normal)
            _logContent += $"[{status.ToString().ToUpper()}] ";
        _logContent += $"{message}{Environment.NewLine}";
        if (useSeparator)
            _logContent += Environment.NewLine;
        Logging?.Invoke(message, status);
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
