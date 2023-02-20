using System;

namespace binstarjs03.AerialOBJ.MvvmAppCore.Services;
public interface ILogService
{
    string LogContent { get; }

    event Action Logging;

    void Clear();
    void Log(string message, bool useSeparator = false);
    void Log(string message, LogStatus status, bool useSeparator = false);
    void LogRuntimeInfo();
    void LogException(string message, Exception e, string? operationAbortedMessage = null);
}

public enum LogStatus
{
    Normal,
    Warning,
    Error,
    Success,
    Aborted,
}