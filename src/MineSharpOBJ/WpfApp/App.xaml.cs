using System;
using System.Windows;
using System.Windows.Threading;

namespace binstarjs03.MineSharpOBJ.WpfApp;

public partial class App : Application {
	public App() {
		LauchTime = DateTime.Now;
	}

    public static DateTime LauchTime { get; set; }

	private void OnStartup(object sender, StartupEventArgs e)
	{
		Current.DispatcherUnhandledException += OnUnhandledException;
	}

	private void OnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
	{
		string msg = $"An unhandled exception occured: \n"
				   + $"{e.Exception}\n"
				   + $"{e.Exception.Message}\n\n";

        MessageBox.Show(msg, "MineSharpOBJ Crashed", MessageBoxButton.OK, MessageBoxImage.Error);
	}
}
