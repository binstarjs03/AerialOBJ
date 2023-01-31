using System;
using System.Collections.Generic;
using System.IO;

using binstarjs03.AerialOBJ.Imaging.ChunkRendering;
using binstarjs03.AerialOBJ.WpfApp.Factories;
using binstarjs03.AerialOBJ.WpfApp.Models.Settings;
using binstarjs03.AerialOBJ.WpfApp.Services;
using binstarjs03.AerialOBJ.WpfApp.Services.ChunkRegionManaging;
using binstarjs03.AerialOBJ.WpfApp.Services.ChunkRendering;
using binstarjs03.AerialOBJ.WpfApp.Services.Dispatcher;
using binstarjs03.AerialOBJ.WpfApp.Services.Input;
using binstarjs03.AerialOBJ.WpfApp.Services.IOService;
using binstarjs03.AerialOBJ.WpfApp.Services.IOService.SavegameLoader;
using binstarjs03.AerialOBJ.WpfApp.Services.ModalServices;
using binstarjs03.AerialOBJ.WpfApp.ViewModels;
using binstarjs03.AerialOBJ.WpfApp.Views;

using Microsoft.Extensions.DependencyInjection;

namespace binstarjs03.AerialOBJ.WpfApp;
internal static class ServiceConfiguration
{
    // For clarity, do not remove explicit type parameter for adding service!
    internal static IServiceProvider Configure(string[] args)
    {
        IServiceCollection services = new ServiceCollection();

        services.ConfigureApplicationWideSingleton(args);
        services.ConfigureFactories();
        services.ConfigureViews();
        services.ConfigureViewModels();
        services.ConfigureServices();

        return services.BuildServiceProvider();
    }

    internal static void ConfigureApplicationWideSingleton(this IServiceCollection services, string[] args)
    {
        services.AddSingleton<AppInfo>(x => new AppInfo
        {
            AppName = "AerialOBJ",
            Version = "1.0.0",
            LaunchTime = DateTime.Now,
            Arguments = args,
        });
        services.AddSingleton<ConstantPath>(x =>
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            return new ConstantPath()
            {
                CurrentPath = currentPath,
                DefinitionsPath = Path.Combine(currentPath, "Definitions"),
                SettingPath = Path.Combine(currentPath, "settings.json"),
            };
        });
        services.AddSingleton<GlobalState>();
        services.AddSingleton<Setting>(x =>
        {
            IChunkShaderRepository shaderRepository = x.GetRequiredService<IChunkShaderRepository>();
            return new Setting
            {
                DefinitionSetting = DefinitionSetting.GetDefaultSetting(),
                PerformanceSetting = PerformanceSetting.GetDefaultSetting(),
                ViewportSetting = ViewportSetting.GetDefaultSetting(shaderRepository),
            };
        });
        services.AddSingleton<ViewState>();
    }

    internal static void ConfigureFactories(this IServiceCollection services)
    {
        services.AddSingleton<RegionDataImageModelFactory>();
        services.AddSingleton<IRegionImageFactory, RegionImageFactory>();
    }

    internal static void ConfigureViews(this IServiceCollection services)
    {
        services.AddSingleton<MainView>();
        services.AddSingleton<DebugLogView>();

        services.AddTransient<AboutView>();
        services.AddTransient<DefinitionManagerView>();
        services.AddTransient<NewDefinitionManagerView>();
        services.AddTransient<ViewportView>();
        services.AddTransient<SettingView>();
    }

    internal static void ConfigureViewModels(this IServiceCollection services)
    {
        services.AddSingleton<AbstractViewModel>();
        services.AddSingleton<MainViewModel>();
        services.AddSingleton<DebugLogViewModel>();

        services.AddTransient<ViewportViewModel>();
        services.AddTransient<ViewportViewModelInputHandler>();
        services.AddTransient<DefinitionManagerViewModel>();
        services.AddTransient<SettingViewModel>();
    }

    internal static void ConfigureServices(this IServiceCollection services)
    {
        services.AddSingleton<IDispatcher, WpfDispatcher>(x => new WpfDispatcher(App.Current.Dispatcher));
        services.AddSingleton<IModalService, ModalService>(x =>
        {
            IDialogView aboutViewFactory() => x.GetRequiredService<AboutView>();
            IDialogView definitionManagerViewFactory() => x.GetRequiredService<NewDefinitionManagerView>();
            IDialogView settingViewFactory() => x.GetRequiredService<SettingView>();
            return new ModalService(aboutViewFactory, definitionManagerViewFactory, settingViewFactory);
        });
        services.AddSingleton<IDefinitionManager, DefinitionManager>();
        services.AddSingleton<ILogService, LogService>();
        services.AddSingleton<IAbstractIO, AbstractIO>();
        services.AddSingleton<IRegionDiskLoader, RegionDiskLoader>();
        services.AddSingleton<ISavegameLoader, SavegameLoader>();
        services.AddSingleton<IDefinitionIO, DefinitionIO>();

        services.AddTransient<IChunkRegionManager, ChunkRegionManager>();
        services.AddTransient<IChunkLoadingPattern, RandomChunkLoadingPattern>();
        services.AddSingleton<IChunkRenderer, ChunkRenderer>(x =>
        {
            Setting setting = x.GetRequiredService<Setting>();
            return new ChunkRenderer(setting.DefinitionSetting, setting.ViewportSetting);
        });
        services.AddTransient<IKeyHandler, KeyHandler>();
        services.AddTransient<IMouseHandler, MouseHandler>();
        services.AddSingleton<IChunkShaderRepository, ChunkShaderRepository>(x =>
        {
            IChunkShader flat = new FlatChunkShader();
            IChunkShader standard = new StandardChunkShader();
            Dictionary<string, IChunkShader> shaders = new()
            {
                { "Flat", flat },
                { "Standard", standard },
            };
            return new ChunkShaderRepository(shaders, standard);
        });
    }
}
