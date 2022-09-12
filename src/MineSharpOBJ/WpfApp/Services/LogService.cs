using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

    public static void Log() {
        s_logHandler?.Invoke("");
    }

    public static void LogSeparator(string content) {
        Log(content);
        Log();
    }

    public static void Log(string content) {
        s_logHandler?.Invoke(content);
    }

    public static void LogWarning(string content, bool pushNotification = false) {
        s_logHandler?.Invoke($"--WARNING--: {content}");
        if (pushNotification)
            PushNotification(content);
    }

    public static void LogError(string content, bool pushNotification = false) {
        s_logHandler?.Invoke($"--ERROR--: {content}");
        if (pushNotification)
            PushNotification(content);
    }

    public static void LogNotification(string content, bool pushNotification = true) {
        s_logHandler?.Invoke($"--NOTIFICATION--: {content}");
        if (pushNotification)
            PushNotification(content);
    }

    public static void PushNotification(string message) {
        s_notificationHandler?.Invoke($"{message}");
    }
}