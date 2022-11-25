using System;
using System.Windows;
using System.Windows.Threading;

using binstarjs03.AerialOBJ.WpfAppNew.Services;
using binstarjs03.AerialOBJ.WpfAppNew.View;

namespace binstarjs03.AerialOBJ.WpfAppNew;

public partial class App : Application
{
    public event StartupEventHandler? Initializing;
    public new static App Current => (Application.Current as App)!;

    protected override void OnStartup(StartupEventArgs e)
    {
        ShutdownMode = ShutdownMode.OnMainWindowClose;
        DebugLogWindow debugLogWindow = new();
        MainWindow = new MainWindow();
        MainWindow.Show();
        debugLogWindow.Owner = MainWindow;
        (MainWindow as MainWindow)!.RequestSynchronizeWindowPosition += 
            debugLogWindow.OnSynchronizeWindowPositionRequested;
        (MainWindow as MainWindow)!.SynchronizeWindowPosition();

        LogService.LogRuntimeInfo();
        LogService.Log("Starting Initialization...");
        Initializing?.Invoke(this, e);
        LogService.Log("Initialization complete", useSeparator: true);
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
}
