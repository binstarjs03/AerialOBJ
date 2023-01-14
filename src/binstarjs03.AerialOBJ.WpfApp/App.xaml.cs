using System;
using System.Windows;

using binstarjs03.AerialOBJ.WpfApp.Services;
using binstarjs03.AerialOBJ.WpfApp.Views;

using Microsoft.Extensions.DependencyInjection;

namespace binstarjs03.AerialOBJ.WpfApp;
public partial class App : Application
{
    public IServiceProvider ServiceProvider { get; } = ServiceConfiguration.Configure();
    public static new App Current => (Application.Current as App)!;

    protected override void OnStartup(StartupEventArgs e)
    {
        ShutdownMode = ShutdownMode.OnMainWindowClose;

        SaveCommandLineArguments(e.Args);

        MainWindow = GetMainWindow();
        MainWindow.Show();
        if (MainWindow is MainView mainView)
            ConfigureDebugLogWindow(mainView);

        InitializeLogService();
        InitializeViewState();
        InitializeDefinitions();
    }

    private MainView GetMainWindow() => ServiceProvider.GetRequiredService<MainView>();

    private void ConfigureDebugLogWindow(MainView mainView)
    {
        DebugLogView debugLogView = ServiceProvider.GetRequiredService<DebugLogView>();
        debugLogView.Owner = mainView;
        mainView.RequestSetDebugViewPosition += debugLogView.SetTopLeft;
        mainView.SyncDebugViewPosition();
    }

    private void SaveCommandLineArguments(string[] args)
    {
        ServiceProvider.GetRequiredService<GlobalState>().Arguments = args;
    }

    private void InitializeLogService()
    {
        ILogService logService = ServiceProvider.GetRequiredService<ILogService>();
        logService.LogRuntimeInfo();
    }

    private void InitializeViewState()
    {
        GlobalState globalState = ServiceProvider.GetRequiredService<GlobalState>();

        // immediately set debug log window to visible if debug enabled
        if (globalState.IsDebugEnabled)
        {
            ViewState viewState = ServiceProvider.GetRequiredService<ViewState>();
            viewState.IsDebugLogViewVisible = true;
        }
    }

    private void InitializeDefinitions()
    {
        IDefinitionManagerService definitionManagerService = ServiceProvider.GetRequiredService<IDefinitionManagerService>();
        definitionManagerService.LoadDefinitionFolder();
    }
}
