using System;
using System.Windows;

using binstarjs03.AerialOBJ.MVVM.ViewModels;
using binstarjs03.AerialOBJ.WpfApp.Views;

using Microsoft.Extensions.DependencyInjection;

namespace binstarjs03.AerialOBJ.WpfApp;
public partial class App : Application
{
    public static new App Current => (Application.Current as App)!;

#pragma warning disable CS8618 // nullable warning
    public IServiceProvider ServiceProvider { get; private set; }
#pragma warning restore CS8618

    protected override void OnStartup(StartupEventArgs e)
    {
        ShutdownMode = ShutdownMode.OnMainWindowClose;
        ServiceProvider = ServiceConfiguration.Configure(e.Args);
        MainWindow = ServiceProvider.GetRequiredService<MainWindow>();
        MainWindow.Show();
    }
}
