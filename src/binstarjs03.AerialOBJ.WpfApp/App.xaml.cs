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
#if RELEASE // don't handle exception on debug version, instead 
                   // inspect it in the IDE if we want to debug it
        Current.DispatcherUnhandledException += OnUnhandledException;
#endif
        ShutdownMode = ShutdownMode.OnMainWindowClose;

        DebugLogWindow debugLogWindow = new();
        MainWindow mainWindow = new(debugLogWindow);
        MainWindow = mainWindow;
        MainWindow.Show();
        debugLogWindow.Owner = MainWindow;

#if DEBUG
        State.DebugLogWindowVisible = true;
#endif

        LogService.LogRuntimeInfo();
        LogService.Log("Starting Initialization...");
        Initializing?.Invoke(this, e);
        IOService.RegisterSavegamePath(mainWindow);
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
