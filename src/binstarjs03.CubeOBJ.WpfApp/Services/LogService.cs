using System;

namespace binstarjs03.AerialOBJ.WpfApp.Services;

public class LogService
{
    private static Action<string>? s_logHandler;
    private static Action<string>? s_notificationHandler;
    private static Func<string>? s_getLogContent;

    public static Action<string>? LogHandlers
    {
        get => s_logHandler;
        set => s_logHandler = value;
    }

    public static Action<string>? NotificationHandlers
    {
        get => s_notificationHandler;
        set => s_notificationHandler = value;
    }

    public static Func<string>? GetLogContentHandlers
    {
        get => s_getLogContent;
        set => s_getLogContent = value;
    }

    /// <summary>
    /// Write log content to disk upon crashed (should not be called manually)
    /// </summary>
    /// <param name="path">Disk path location to write the file</param>
    /// <returns>Report whether writing is successful or failure</returns>
    public static bool WriteLogToDiskOnCrashed(string path)
    {
        try
        {
            string logContent = GetLogContentHandlers?.Invoke()!;
            IOService.WriteText(path, logContent);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static void LogNewline()
    {
        s_logHandler?.Invoke("");
    }

    public static void Log(string content, bool useSeparator = false)
    {
        s_logHandler?.Invoke(content);
        if (useSeparator)
            LogNewline();
    }

    public static void LogWarning(string content, bool pushNotification = false, bool useSeparator = false)
    {
        LogEmphasis(content, "WARNING", pushNotification, useSeparator);
    }

    public static void LogError(string content, bool pushNotification = false, bool useSeparator = false)
    {
        LogEmphasis(content, "ERROR", pushNotification, useSeparator);
    }

    public static void LogNotification(string content, bool pushNotification = true, bool useSeparator = false)
    {
        LogEmphasis(content, "NOTIFICATION", pushNotification, useSeparator);
    }

    public static void LogSuccess(string content, bool pushNotification = true, bool useSeparator = false)
    {
        LogEmphasis(content, "SUCCESS", pushNotification, useSeparator);
    }

    public static void LogAborted(string content, bool pushNotification = true, bool useSeparator = false)
    {
        LogEmphasis(content, "ABORTED", pushNotification, useSeparator);
    }

    private static void LogEmphasis(string content, string emphasisWord, bool pushNotification = true, bool useSeparator = false)
    {
        s_logHandler?.Invoke($"--{emphasisWord}--: {content}");
        if (pushNotification)
            PushNotification(content);
        if (useSeparator)
            LogNewline();
    }

    public static void PushNotification(string message)
    {
        s_notificationHandler?.Invoke($"{message}");
    }

    public static void LogRuntimeInfo()
    {
        Log($"Launch time: {App.LauchTime}");
        Log("MineSharpOBJ Version: ...");
        Log("Commit Hash: ...", useSeparator: true);

        Log($"Host OS: {Environment.OSVersion}");
        Log($"CPU cores count: {Environment.ProcessorCount}");
        Log($".NET Runtime Version: {Environment.Version}", useSeparator: true);
    }
}

