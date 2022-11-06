using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Threading;

using binstarjs03.AerialOBJ.WpfApp.Services;
using binstarjs03.AerialOBJ.WpfApp.UIElements.Windows;

namespace binstarjs03.AerialOBJ.WpfApp;

public partial class App : Application
{
    public new static App Current => (Application.Current as App)!;
    public AppState State { get; }

    /// <summary>
    /// Invoked after all user interface are loaded
    /// </summary>
    public event StartupEventHandler? Initializing;

    public App()
    {
        State = new AppState();
    }

    public static void InvokeDispatcher(Action method, DispatcherPriority priority, DispatcherSynchronization synchronization)
    {
        switch (synchronization)
        {
            case DispatcherSynchronization.Synchronous:
                Current?.Dispatcher.Invoke(method, priority);
                break;
            case DispatcherSynchronization.Asynchronous:
                Current?.Dispatcher.BeginInvoke(method, priority);
                break;
            default:
                throw new NotImplementedException();
        }
    }

    public static T InvokeDispatcherSynchronous<T>(Func<T> method, DispatcherPriority priority)
    {
        return Current.Dispatcher.Invoke(method, priority);
    }

    public new static void VerifyAccess()
    {
        Current?.Dispatcher.VerifyAccess();
    }

    public new static bool CheckAccess()
    {
        if (Current is not null)
            return Current.Dispatcher.CheckAccess();
        return false;
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        //Current.DispatcherUnhandledException += OnUnhandledException;
        ShutdownMode = ShutdownMode.OnMainWindowClose;

        DebugLogWindow debugLogWindow = new();
        MainWindow = new MainWindow(debugLogWindow);
        MainWindow.Show();
        debugLogWindow.Owner = MainWindow;

        LogService.LogRuntimeInfo();
        LogService.Log("Starting Initialization...");
        Initializing?.Invoke(this, e);
        LogService.Log("Initialization complete", useSeparator: true);
    }

    private void OnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        string modalTitle = $"{AppState.AppName} Crashed";
        string msg = $"An unhandled exception occured: \n"
                   + $"{e.Exception}\n"
                   + $"{e.Exception.Message}\n\n";
        LogService.Log(msg);
        MessageBox.Show(msg, modalTitle, MessageBoxButton.OK, MessageBoxImage.Error);

        string lauchTime = State.LaunchTime.ToString().Replace('/', '-').Replace(':', '-');
        string path = $"{Environment.CurrentDirectory}/{AppState.AppName} Crash Log {lauchTime}.txt";

        string logSaveMsg;
        MessageBoxImage messageBoxIcon;
        if (LogService.WriteLogToDiskOnCrashed(path))
        {
            logSaveMsg = $"Debug log content has been saved to {path}";
            messageBoxIcon = MessageBoxImage.Information;
        }
        else
        {
            logSaveMsg = $"Failed writing Debug log content to {path}";
            messageBoxIcon = MessageBoxImage.Warning;
        }
        MessageBox.Show(logSaveMsg, modalTitle, MessageBoxButton.OK, messageBoxIcon);
    }
}
