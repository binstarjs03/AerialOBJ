using System;
using System.Windows;

using binstarjs03.AerialOBJ.WpfApp.Services;
using binstarjs03.AerialOBJ.WpfApp.ViewModels;
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

        MainWindow = GetAndConfigureMainWindow();
        MainWindow.Show();
        if (MainWindow is MainView mainView)
            ConfigureDebugLogWindow(mainView);

        InitializeLogService();
        InitializeViewState();
        InitializeDefinitions();
    }

    private Window GetAndConfigureMainWindow()
    {
        MainView mainView = ServiceProvider.GetRequiredService<MainView>();
        MainViewModel mainViewModel = ServiceProvider.GetRequiredService<MainViewModel>();
        mainViewModel.RequestCloseView += mainView.Close;
        return mainView;
    }

    private void ConfigureDebugLogWindow(MainView mainView)
    {
        DebugLogView debugLogView = ServiceProvider.GetRequiredService<DebugLogView>();
        debugLogView.Owner = mainView;
        mainView.DebugViewSyncPositionRequested += debugLogView.SetTopLeft;
        mainView.RequestDebugViewsSyncPosition();
    }

    private void InitializeLogService()
    {
        ILogService logService = ServiceProvider.GetRequiredService<ILogService>();
        logService.LogRuntimeInfo();
    }

    private void InitializeViewState()
    {
#if DEBUG // TODO refactor if "debug" is in command-line arg
        ViewState viewState = ServiceProvider.GetRequiredService<ViewState>();
        viewState.IsDebugLogWindowVisible = true;
#endif
    }

    private void InitializeDefinitions()
    {
        IDefinitionManagerService definitionManagerService = ServiceProvider.GetRequiredService<IDefinitionManagerService>();
        definitionManagerService.LoadDefinitionFolder();
    }
}
