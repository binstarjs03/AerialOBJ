using System.Windows;

using Autofac;

using binstarjs03.AerialOBJ.WpfAppNew2.Views;

namespace binstarjs03.AerialOBJ.WpfAppNew2;
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        ShutdownMode = ShutdownMode.OnMainWindowClose;
        IContainer container = ContainerConfig.Configure();
        MainWindow = container.Resolve<MainWindow>();
        MainWindow.Show();
    }
}
