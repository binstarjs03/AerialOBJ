using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Windows;

using binstarjs03.AerialOBJ.Core.Definitions;
using binstarjs03.AerialOBJ.WpfApp.Models.Settings;
using binstarjs03.AerialOBJ.WpfApp.Services;
using binstarjs03.AerialOBJ.WpfApp.Services.ChunkRegionManaging.Patterns;
using binstarjs03.AerialOBJ.WpfApp.Services.ChunkRendering;
using binstarjs03.AerialOBJ.WpfApp.Services.IOService;
using binstarjs03.AerialOBJ.WpfApp.Services.ModalServices;
using binstarjs03.AerialOBJ.WpfApp.Views;

using Microsoft.Extensions.DependencyInjection;

namespace binstarjs03.AerialOBJ.WpfApp;
public partial class App : Application
{
#pragma warning disable CS8618 // nullable warning
    public static new App Current => (Application.Current as App)!;
    public IServiceProvider ServiceProvider { get; private set; }
#pragma warning restore CS8618

    protected override void OnStartup(StartupEventArgs e)
    {
        ShutdownMode = ShutdownMode.OnMainWindowClose;
        ServiceProvider = ServiceConfiguration.Configure(e.Args);

        InitializeLogService();
        LoadDefinitions();
        LoadSettings();
        InitializeViewState();

        MainWindow = GetMainWindow();
        MainWindow.Show();
        ConfigureDebugLogWindow((MainWindow as MainView)!);
    }

    private MainView GetMainWindow() => ServiceProvider.GetRequiredService<MainView>();

    private void ConfigureDebugLogWindow(MainView mainView)
    {
        DebugLogView debugLogView = ServiceProvider.GetRequiredService<DebugLogView>();
        debugLogView.Owner = mainView;
        mainView.RequestSetDebugViewPosition += debugLogView.SetTopLeft;
        mainView.SyncDebugViewPosition();
    }

    private void InitializeLogService()
    {
        ILogService logService = ServiceProvider.GetRequiredService<ILogService>();
        logService.LogRuntimeInfo();
    }

    private void InitializeViewState()
    {
        // immediately set debug log window to visible if debug enabled
        AppInfo appInfo = ServiceProvider.GetRequiredService<AppInfo>();
        if (appInfo.IsDebugEnabled)
        {
            ViewState viewState = ServiceProvider.GetRequiredService<ViewState>();
            viewState.IsDebugLogViewVisible = true;
        }
    }

    private void LoadDefinitions()
    {
        IDefinitionManager definitionManager = ServiceProvider.GetRequiredService<IDefinitionManager>();
        IDefinitionIO definitionIO = ServiceProvider.GetRequiredService<IDefinitionIO>();

        ILogService logService = ServiceProvider.GetRequiredService<ILogService>();
        IModalService modalService = ServiceProvider.GetRequiredService<IModalService>();

        bool hasErrorMessageBoxShown = false;

        List<IRootDefinition> definitions = definitionIO.LoadDefinitionFolder(exceptionHandler);
        foreach (IRootDefinition definition in definitions)
            definitionManager.LoadDefinition(definition);

        void exceptionHandler(Exception e, string definitionFilename)
        {
            string caption = "Cannot load definition";
            logService.LogException($"{caption} {definitionFilename}", e);
            if (hasErrorMessageBoxShown)
                return;
            modalService.ShowErrorMessageBox(new MessageBoxArg()
            {
                Caption = caption,
                Message = $"An exception occured during loading definition {definitionFilename}.\n" +
                          $"See the Debug Log window for detailed information.\n" +
                          $"Any further exception during definition folder loading will be logged to Debug Log window"
            });
            hasErrorMessageBoxShown = true;
        }
    }

    private void LoadSettings()
    {
        Setting setting = ServiceProvider.GetRequiredService<Setting>();
        ConstantPath path = ServiceProvider.GetRequiredService<ConstantPath>();
        IChunkShaderRepository shaderRepo = ServiceProvider.GetRequiredService<IChunkShaderRepository>();
        IChunkLoadingPatternRepository chunkLoadingPatternRepo = ServiceProvider.GetRequiredService<IChunkLoadingPatternRepository>();
        IDefinitionManager definitionManager = ServiceProvider.GetRequiredService<IDefinitionManager>();

        ILogService logService = ServiceProvider.GetRequiredService<ILogService>();
        IModalService modalService = ServiceProvider.GetRequiredService<IModalService>();

        string settingPath = path.SettingPath;
        if (!File.Exists(settingPath))
        {
            try
            {
                SettingIO.SaveSetting(settingPath, setting);
            }
            catch (Exception e) { handleSavingException(e); }
            return;
        }
        try
        {
            SettingIO.LoadSetting(setting, settingPath, definitionManager, shaderRepo, chunkLoadingPatternRepo);
        }
        catch (JsonException e) { handleLoadingParsingException(e); }
        catch (Exception e) { handleLoadingIOException(e); }

        void handleSavingException(Exception e)
        {
            string caption = "Cannot save setting";
            logService.LogException(caption, e);
            modalService.ShowWarningMessageBox(new MessageBoxArg
            {
                Caption = caption,
                Message = $"An exception occured when saving setting:\n{e}",
            });
        }

        void handleLoadingIOException(Exception e)
        {
            string caption = "Cannot read setting";
            logService.LogException(caption, e);
            modalService.ShowWarningMessageBox(new MessageBoxArg
            {
                Caption = caption,
                Message = $"An exception occured when reading setting:\n{e}"
            });
        }

        void handleLoadingParsingException(Exception e)
        {
            string caption = "Cannot read setting";
            logService.LogException(caption, e);
            modalService.ShowWarningMessageBox(new MessageBoxArg
            {
                Caption = caption,
                Message = $"An exception occured when parsing setting, "
                        + $"this may indicate the setting.json content have bad syntax:\n{e}"
            });
        }
    }
}
