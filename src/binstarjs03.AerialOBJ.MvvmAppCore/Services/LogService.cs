using System;

namespace binstarjs03.AerialOBJ.MvvmAppCore.Services;

// TODO refactor string _logContent member into more efficient one,
// maybe stream-based buffer, or stringbuilder, etc
public class LogService : ILogService
{
    private string _logContent = "";
    private readonly AppInfo _appInfo;

    public LogService(AppInfo appInfo)
    {
        _appInfo = appInfo;
    }

    public string LogContent => _logContent;

    public event Action? Logging;

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
        Logging?.Invoke();
    }

    public void LogRuntimeInfo()
    {
        Log($"Launch time: {_appInfo.LaunchTime}");
        Log($"{_appInfo.AppName} Version: ...");
        Log("Commit Hash: ...", useSeparator: true);

        Log($"Host OS: {Environment.OSVersion}");
        Log($"CPU cores count: {Environment.ProcessorCount}");
        Log($".NET Runtime Version: {Environment.Version}", useSeparator: true);
    }

    public void LogException(string message, Exception e, string? operationAbortedMessage = null)
    {
        Log(e.Message, LogStatus.Error);
        Log("Exception Details:");
        Log(e.ToString());
        if (operationAbortedMessage is not null)
            Log(operationAbortedMessage, LogStatus.Aborted, true);
        else
            Log(""); // log newline separator
    }
}
