using System.Windows;

using binstarjs03.AerialOBJ.WpfAppNew.Services;
using binstarjs03.AerialOBJ.WpfAppNew.View;

namespace binstarjs03.AerialOBJ.WpfAppNew;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        ShutdownMode = ShutdownMode.OnMainWindowClose;
        DebugLogWindow debugLogWindow = new();
        MainWindow = new MainWindow();
        MainWindow.Show();
        debugLogWindow.Owner = MainWindow;

        LogService.LogRuntimeInfo();
    }
}
