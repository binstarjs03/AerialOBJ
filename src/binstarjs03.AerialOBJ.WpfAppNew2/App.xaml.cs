using System.Windows;

using binstarjs03.AerialOBJ.WpfAppNew2.Views;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace binstarjs03.AerialOBJ.WpfAppNew2;
public partial class App : Application
{
    public IHost Host { get; private set; }

    public App()
    {
        Host = AppHost.Configure();
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        await Host.StartAsync();

        ShutdownMode = ShutdownMode.OnMainWindowClose;
        MainWindow = Host.Services.GetRequiredService<MainView>();
        MainWindow.Show();

        DebugLogView debugLogView = Host.Services.GetRequiredService<DebugLogView>();
        debugLogView.Owner = MainWindow;
        (MainWindow as MainView)!.DebugViewSetPositionRequested += debugLogView.SetTopLeft;
        (MainWindow as MainView)!.InvokeDebugViewSetPositionRequested();
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        await Host.StopAsync(); 
    }
}
