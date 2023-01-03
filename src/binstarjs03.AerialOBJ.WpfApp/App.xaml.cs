using System;
using System.Windows;

using binstarjs03.AerialOBJ.WpfApp.Components;
using binstarjs03.AerialOBJ.WpfApp.Services;
using binstarjs03.AerialOBJ.WpfApp.Views;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace binstarjs03.AerialOBJ.WpfApp;
public partial class App : Application
{
    public IHost Host { get; } = AppHost.Configure();
    public static new App Current => (Application.Current as App)!;

    protected override async void OnStartup(StartupEventArgs e)
    {
        await Host.StartAsync();

        ShutdownMode = ShutdownMode.OnMainWindowClose;
        MainView mainView = Host.Services.GetRequiredService<MainView>();
        MainWindow = mainView;
        MainWindow.Show();

        DebugLogView debugLogView = Host.Services.GetRequiredService<DebugLogView>();
        debugLogView.Owner = mainView;
        mainView.DebugViewSyncPositionRequested += debugLogView.SetTopLeft;
        mainView.RequestDebugViewsSyncPosition();

        ILogService logService = Host.Services.GetRequiredService<ILogService>();
        logService.LogRuntimeInfo();

#if DEBUG
        ViewState viewState = Host.Services.GetRequiredService<ViewState>();
        viewState.IsDebugLogWindowVisible = true;
#endif
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        await Host.StopAsync();
    }
}
