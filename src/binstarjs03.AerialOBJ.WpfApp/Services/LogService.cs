using System;

namespace binstarjs03.AerialOBJ.WpfApp.Services;

public static class LogService
{
    public static event StringAction? Logging;
    public static event StringAction? NotificationPushed;
    public delegate void StringAction(string content);
    private static string s_logContent = "";
    private static string s_notificationContent = "";

    public static string GetLogContent()
    {
        return s_logContent;
    }

    public static string GetNotificationContent()
    {
        return s_notificationContent;
    }

    public static void ClearLogContent()
    {
        s_logContent = "";
    }

    public static void ClearNotificationContent()
    {
        s_notificationContent = "";
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
            IOService.WriteText(path, s_logContent);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static void LogNewline()
    {
        Log("");
    }

    public static void Log(string content, bool useSeparator = false)
    {
        s_logContent += $"{content}{Environment.NewLine}";
        if (useSeparator)
            LogNewline();
        Logging?.Invoke(content);
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
        Log($"--{emphasisWord}--: {content}", useSeparator);
        if (pushNotification)
            PushNotification(content);
    }

    public static void PushNotification(string message)
    {
        s_notificationContent = message;
        NotificationPushed?.Invoke(message);
    }

    public static void LogRuntimeInfo()
    {
        Log($"Launch time: {App.Current.State.LaunchTime}");
        Log($"{AppState.AppName} Version: ...");
        Log("Commit Hash: ...", useSeparator: true);

        Log($"Host OS: {Environment.OSVersion}");
        Log($"CPU cores count: {Environment.ProcessorCount}");
        Log($".NET Runtime Version: {Environment.Version}", useSeparator: true);
    }
}

