using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Linq;

using binstarjs03.AerialOBJ.Core.Definitions;
using binstarjs03.AerialOBJ.Core.IniFormat;
using binstarjs03.AerialOBJ.WpfApp.Services;
using binstarjs03.AerialOBJ.WpfApp.Services.IOService;
using binstarjs03.AerialOBJ.WpfApp.Services.ModalServices;
using binstarjs03.AerialOBJ.WpfApp.Views;

using Microsoft.Extensions.DependencyInjection;
using binstarjs03.AerialOBJ.WpfApp.Settings;

namespace binstarjs03.AerialOBJ.WpfApp;
public partial class App : Application
{
#pragma warning disable CS8618 // nullable warning
    public static new App Current => (Application.Current as App)!;
    public IServiceProvider ServiceProvider { get; private set; }
    public GlobalState GlobalState { get; private set; }
#pragma warning restore CS8618

    protected override void OnStartup(StartupEventArgs e)
    {
        ShutdownMode = ShutdownMode.OnMainWindowClose;

        SettingState setting = SettingState.GetDefaultSetting();
        GlobalState = ConfigureGlobalState(e.Args, setting);
        ServiceProvider = ServiceConfiguration.Configure(GlobalState);

        MainWindow = GetMainWindow();
        MainWindow.Show();
        if (MainWindow is MainView mainView)
            ConfigureDebugLogWindow(mainView);

        InitializeLogService();
        InitializeViewState();
        LoadDefinitions();
        LoadSettings();
    }

    private static GlobalState ConfigureGlobalState(string[] args, SettingState setting)
    {
        DateTime lauchTime = DateTime.Now;
        string version = "Alpha";
        string currentPath = AppDomain.CurrentDomain.BaseDirectory;
        string definitionsPath = Path.Combine(currentPath, "Definitions");
        return new GlobalState(lauchTime, version, currentPath, definitionsPath, args, setting);
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
        if (GlobalState.IsDebugEnabled)
        {
            ViewState viewState = ServiceProvider.GetRequiredService<ViewState>();
            viewState.IsDebugLogViewVisible = true;
        }
    }

    // TODO we may move out the logic of this method into separate class, maybe "IDefinitionInitializer"
    // load all definitions in definition folder
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

    // TODO refactor binding IniDocument values into C# instance, do it automatically using attributes etc
    private void LoadSettings()
    {
        ILogService logService = ServiceProvider.GetRequiredService<ILogService>();
        IModalService modalService = ServiceProvider.GetRequiredService<IModalService>();
        IDefinitionManager definitionManager = ServiceProvider.GetRequiredService<IDefinitionManager>();

        string settingPath = Path.Combine(GlobalState.CurrentPath, "setting.ini");
        if (!File.Exists(settingPath))
            SettingIO.SaveDefaultSetting(settingPath);

        string settingContent = File.ReadAllText(settingPath);
        IniDocument settingDoc = IniDeserializing.Deserialize(settingContent);

        if (settingDoc.Subsections.TryGetValue("DefinitionSetting", out IniSection? definitionSettingDoc))
            if (definitionSettingDoc.Properties.TryGetValue("ViewportDefinition", out string? vdName))
                foreach (ViewportDefinition vd in definitionManager.LoadedViewportDefinitions
                                                                   .Where(vd => vd.Name == vdName))
                {
                    GlobalState.Setting.DefinitionSetting.CurrentViewportDefinition = vd;
                    break;
                }

        if (settingDoc.Subsections.TryGetValue("ViewportSetting", out IniSection? viewportSettingDoc))
        {
            if (viewportSettingDoc.Properties.TryGetValue("ChunkShadingStyle", out string? cssValue))
                if (Enum.TryParse(cssValue, out ChunkShadingStyle css))
                    GlobalState.Setting.ViewportSetting.ChunkShadingStyle = css;
            if (viewportSettingDoc.Properties.TryGetValue("ChunkThreads", out string? ctValue))
                if (int.TryParse(ctValue, out int ct))
                    GlobalState.Setting.ViewportSetting.ChunkThreads = int.Clamp(ct, 1, Environment.ProcessorCount);
        }
    }
}
