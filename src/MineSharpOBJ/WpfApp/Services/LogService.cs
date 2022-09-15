using System;
namespace binstarjs03.MineSharpOBJ.WpfApp.Services;

public class LogService {
    public delegate void LogDelegate(string content);
    private static LogDelegate? s_logHandler;
    private static LogDelegate? s_notificationHandler;

    public static LogDelegate? LogHandlers {
        get { return s_logHandler; }
        set { s_logHandler = value; }
    }

    public static LogDelegate? NotificationHandlers {
        get { return s_notificationHandler; }
        set { s_notificationHandler = value; }
    }

    public static void LogNewline() {
        s_logHandler?.Invoke("");
    }

    public static void Log(string content, bool useSeparator = false) {
        s_logHandler?.Invoke(content);
        if (useSeparator)
            LogNewline();
    }

    public static void LogWarning(string content, bool pushNotification = false, bool useSeparator = false) {
        LogEmphasis(content, "WARNING", pushNotification, useSeparator);
    }

    public static void LogError(string content, bool pushNotification = false, bool useSeparator = false) {
        LogEmphasis(content, "ERROR", pushNotification, useSeparator);
    }

    public static void LogNotification(string content, bool pushNotification = true, bool useSeparator = false) {
        LogEmphasis(content, "NOTIFICATION", pushNotification, useSeparator);
    }

    private static void LogEmphasis(string content, string emphasisWord, bool pushNotification = true, bool useSeparator = false) {
        s_logHandler?.Invoke($"--{emphasisWord}--: {content}");
        if (pushNotification)
            PushNotification(content);
        if (useSeparator)
            LogNewline();
    }

    public static void PushNotification(string message) {
        s_notificationHandler?.Invoke($"{message}");
    }

    public static void LogRuntimeInfo() {
        Log($"Launch time: {App.LauchTime}");
        Log("MineSharpOBJ Version: v0.0");
        Log("Commit Hash: 36fa86b5d8eda9bf9a4ea798bfffc3fc07c965e0", useSeparator: true);

        Log($"Host OS: {Environment.OSVersion}");
        Log($"CPU cores count: {Environment.ProcessorCount}");
        Log($".NET Runtime Version: {Environment.Version}", useSeparator: true);
    }
}
