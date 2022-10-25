using System;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

using binstarjs03.AerialOBJ.WpfApp.Services;

namespace binstarjs03.AerialOBJ.WpfApp;

public partial class App : Application
{
    public App()
    {
        LauchTime = DateTime.Now;

        // TODO set max threadPool according to user setting chunk threads!
        int maxThreadPoolCount = Math.Clamp(Environment.ProcessorCount - 1, 1, Environment.ProcessorCount);
        ThreadPool.GetMaxThreads(out _, out int completionPortThreads);
        ThreadPool.SetMinThreads(0, completionPortThreads);
        ThreadPool.SetMaxThreads(maxThreadPoolCount, completionPortThreads);
    }

    public static DateTime LauchTime { get; set; }
    public static string AppName => "AerialOBJ";

    public static void InvokeDispatcher(Action action, DispatcherPriority priority)
    {
        if (Current is null)
            return;
        Current.Dispatcher.Invoke(action, priority);
    }

    public static DispatcherOperation? BeginInvokeDispatcher(Action action, DispatcherPriority priority)
    {
        if (Current is null)
            return null;
        return Current.Dispatcher.BeginInvoke(action, priority);
    }

    private void OnStartup(object sender, StartupEventArgs e)
    {
        //Current.DispatcherUnhandledException += OnUnhandledException;
    }

    private void OnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        string modalTitle = $"{AppName} Crashed";
        string msg = $"An unhandled exception occured: \n"
                   + $"{e.Exception}\n"
                   + $"{e.Exception.Message}\n\n";
        LogService.LogError("Critical Error!");
        LogService.Log(msg);
        MessageBox.Show(msg, modalTitle, MessageBoxButton.OK, MessageBoxImage.Error);

        string lauchTime = LauchTime.ToString().Replace('/', '-').Replace(':', '-');
        string path = $"{Environment.CurrentDirectory}/{AppName} Crash Log {lauchTime}.txt";

        string logSaveMsg;
        if (LogService.WriteLogToDiskOnCrashed(path))
            logSaveMsg = $"Debug log content has been saved to {path}";
        else
            logSaveMsg = $"Failed writing Debug log content to {path}";
        MessageBox.Show(logSaveMsg, modalTitle, MessageBoxButton.OK, MessageBoxImage.Information);
    }
}
