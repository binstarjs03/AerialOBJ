/*
Copyright (c) 2022, Bintang Jakasurya
All rights reserved. 

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System;

namespace binstarjs03.AerialOBJ.WpfAppNew.Services;

public static class LogService
{
    public static event StringAction? Logging;
    public static event StringAction? NotificationPushed;
    public delegate void StringAction(string content);
    private static string s_logContent = "";
    private static string s_notificationContent = "";

    public static string LogContent => s_logContent;

    public static string NotificationContent => s_notificationContent;

    public static void ClearLogContent()
    {
        s_logContent = "";
        LogRuntimeInfo();
    }

    public static void ClearNotificationContent() => s_notificationContent = "";

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

    public static void Log(string content, bool useSeparator = false)
    {
        s_logContent += $"{content}{Environment.NewLine}";
        if (useSeparator)
            LogNewline();
        Logging?.Invoke(content);
    }

    public static void LogNewline() => Log("");

    public static void LogEmphasis(string content, Emphasis emphasis, bool pushNotification = false, bool useSeparator = true)
    {
        Log($"[{emphasis}] {content}", useSeparator);
        if (pushNotification)
            PushNotification(content);
    }

    public enum Emphasis
    {
        Warning,
        Information,
        Error,
        Success,
        Aborted,
    }

    public static void PushNotification(string message)
    {
        s_notificationContent = message;
        NotificationPushed?.Invoke(message);
    }

    public static void LogRuntimeInfo()
    {
        Log($"Launch time: {StateService.LaunchTime}");
        Log($"{StateService.AppName} Version: ...");
        Log("Commit Hash: ...", useSeparator: true);

        Log($"Host OS: {Environment.OSVersion}");
        Log($"CPU cores count: {Environment.ProcessorCount}");
        Log($".NET Runtime Version: {Environment.Version}", useSeparator: true);
    }
}

