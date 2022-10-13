using System;
using System.Windows;
using System.Windows.Threading;

using binstarjs03.MineSharpOBJ.WpfApp.Services;
using binstarjs03.MineSharpOBJ.WpfApp.UIElements;

namespace binstarjs03.MineSharpOBJ.WpfApp;

public partial class App : Application {
	public App() {
		LauchTime = DateTime.Now;
	}

    public static DateTime LauchTime { get; set; }

	private void OnStartup(object sender, StartupEventArgs e)
	{
		Current.DispatcherUnhandledException += OnUnhandledException;
        SharedProperty.SessionInfo = new SessionInfo();
        SharedProperty.IsViewportDebugInfoVisible = true;
        SharedProperty.IsViewportCameraPositionGuideVisible = true;
    }

	private void OnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
	{
		string msg = $"An unhandled exception occured: \n"
				   + $"{e.Exception}\n"
				   + $"{e.Exception.Message}\n\n";
		LogService.LogError("Critical Error!");
		LogService.Log(msg);
        MessageBox.Show(msg, "MineSharpOBJ Crashed", MessageBoxButton.OK, MessageBoxImage.Error);

		string lauchTime = LauchTime.ToString().Replace('/', '-').Replace(':', '-');
        string path = $"{Environment.CurrentDirectory}/MineSharpOBJ Crash Log {lauchTime}.txt";

		string logSaveMsg;
        if (LogService.WriteLogToDiskOnCrashed(path))
			logSaveMsg = $"Debug log content has been saved to {path}";
		else
			logSaveMsg = $"Failed writing Debug log content to {path}";
        MessageBox.Show(logSaveMsg, "MineSharpOBJ Crashed", MessageBoxButton.OK, MessageBoxImage.Information);
	}
}
