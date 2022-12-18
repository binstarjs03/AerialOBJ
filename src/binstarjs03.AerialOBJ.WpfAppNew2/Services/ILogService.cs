using System;

namespace binstarjs03.AerialOBJ.WpfAppNew2.Services;
public interface ILogService
{
    string LogContent { get; }
    event Action Logging;

    void Clear();
    void Log(string message, bool useSeparator = false);
    void LogEmphasis(string message, Emphasis emphasis, bool useSeparator = false);
    void LogRuntimeInfo();
}

public enum Emphasis
{
    Warning,
    Information,
    Error,
    Success,
    Aborted,
}