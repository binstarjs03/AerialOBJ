using System;

namespace binstarjs03.AerialOBJ.WpfApp.Services;
public delegate void LoggingEventHandler(string message, LogStatus status);
public interface ILogService
{
    string LogContent { get; }
    
    event LoggingEventHandler Logging;

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