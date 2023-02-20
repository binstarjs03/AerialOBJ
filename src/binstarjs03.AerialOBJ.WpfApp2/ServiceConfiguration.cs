using System;
using System.IO;

using binstarjs03.AerialOBJ.Imaging.ChunkRendering;
using binstarjs03.AerialOBJ.MvvmAppCore;
using binstarjs03.AerialOBJ.MvvmAppCore.Factories;
using binstarjs03.AerialOBJ.MvvmAppCore.Models.Settings;
using binstarjs03.AerialOBJ.MvvmAppCore.Repositories;
using binstarjs03.AerialOBJ.MvvmAppCore.Services;
using binstarjs03.AerialOBJ.MvvmAppCore.Services.ChunkLoadingPatterns;
using binstarjs03.AerialOBJ.MvvmAppCore.Services.Diagnostics;
using binstarjs03.AerialOBJ.MvvmAppCore.Services.Dispatcher;
using binstarjs03.AerialOBJ.MvvmAppCore.Services.IOService;
using binstarjs03.AerialOBJ.MvvmAppCore.Services.IOService.SavegameLoader;
using binstarjs03.AerialOBJ.MvvmAppCore.Services.ModalServices;
using binstarjs03.AerialOBJ.MvvmAppCore.ViewModels;
using binstarjs03.AerialOBJ.MvvmAppCore.ViewTraits;
using binstarjs03.AerialOBJ.WpfApp.Factories;
using binstarjs03.AerialOBJ.WpfApp.Services;
using binstarjs03.AerialOBJ.WpfApp.Services.Dispatcher;
using binstarjs03.AerialOBJ.WpfApp.Views;

using Microsoft.Extensions.DependencyInjection;

namespace binstarjs03.AerialOBJ.WpfApp;

public static class ServiceConfiguration
{
    public static IServiceProvider Configure(string[] args)
    {
        IServiceCollection services = new ServiceCollection();
        ConfigureCommonSingletons(services, args);
        services.ConfigureViews();
        services.ConfigureViewModels();
        services.ConfigureServices();
        services.ConfigureFactories();
        services.ConfigureRepositories();
        return services.BuildServiceProvider();
    }

    private static void ConfigureCommonSingletons(this IServiceCollection services, string[] args)
    {
        services.AddSingleton<AppInfo>(x => new AppInfo
        {
            AppName = "AerialOBJ",
            Arguments = args,
            LaunchTime= DateTime.Now,
            Version = "...",
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
        services.AddSingleton<SharedViewModelState>();
        services.AddSingleton<Setting>(x =>
        {
            var shaderRepo = x.GetRequiredService<IRepository<IChunkShader>>();
            var chunkLoadingPatternRepo = x.GetRequiredService<IRepository<IChunkLoadingPattern>>();
            return new Setting
            {
                DefinitionSetting = DefinitionSetting.GetDefaultSetting(),
                PerformanceSetting = PerformanceSetting.GetDefaultSetting(),
                ViewportSetting = ViewportSetting.GetDefaultSetting(shaderRepo, chunkLoadingPatternRepo),
            };
        });
    }

    private static void ConfigureViews(this IServiceCollection services)
    {
        services.AddSingleton<MainWindow>();
        services.AddSingleton<DebugLogWindow>();
        services.AddSingleton<ISettablePosition>(x => x.GetRequiredService<DebugLogWindow>());
        services.AddSingleton<ViewportControl>();
        services.AddTransient<GotoWindow>();
        services.AddTransient<SettingWindow>();
        services.AddTransient<DefinitionManagerWindow>();
        services.AddTransient<AboutWindow>();
    }

    private static void ConfigureViewModels(this IServiceCollection services)
    {
        services.AddSingleton<MainViewModel>();
        services.AddSingleton<DebugLogViewModel>();
        services.AddSingleton<MonolithViewportViewModel>();
        services.AddTransient<GotoViewModel>(x =>
        {
            var viewmodel = new GotoViewModel();
            var mainWindowViewModel = x.GetRequiredService<MainViewModel>();
            viewmodel.ClosedRecipient = mainWindowViewModel;
            return viewmodel;
        });
        services.AddTransient<SettingViewModel>();
        services.AddTransient<DefinitionManagerViewModel>();
        services.AddTransient<ClosableViewModel>();
    }

    private static void ConfigureServices(this IServiceCollection services)
    {
        services.AddSingleton<IDispatcher, WpfDispatcher>(x=> new WpfDispatcher(App.Current.Dispatcher));
        services.AddSingleton<ILogService, LogService>();
        services.AddSingleton<IModalService, ModalService>();
        services.AddSingleton<IAbstractIO, AbstractIO>();
        services.AddSingleton<ISavegameLoader, SavegameLoader>();
        services.AddSingleton<IMemoryInfo, MemoryInfo>();
        services.AddSingleton<IDefinitionIO, DefinitionIO>();
        services.AddSingleton<IRegionDiskLoader, RegionDiskLoader>();
        services.AddTransient<IChunkRegionManager, ChunkRegionManager>();
        services.AddSingleton<IChunkRenderer, ChunkRenderer>();
    }

    private static void ConfigureFactories(this IServiceCollection services)
    {
        services.AddSingleton<IRegionDataImageModelFactory, RegionDataImageModelFactory>();
    }

    private static void ConfigureRepositories(this IServiceCollection services)
    {
        services.AddSingleton<IDefinitionRepository, DefinitionRepository>();
        services.AddSingleton<IRepository<IChunkShader>, AbstractRepository<IChunkShader>>(x =>
        {
            var flat = new FlatChunkShader();
            var standard = new StandardChunkShader();
            AbstractRepository<IChunkShader> ret = new(standard);
            ret.Register(flat.ShaderName, flat);
            ret.Register(standard.ShaderName, standard);
            return ret;
        });
        services.AddSingleton<IRepository<IChunkLoadingPattern>, AbstractRepository<IChunkLoadingPattern>>(x =>
        {
            var alternateCheckerboard = new AlternateCheckerboardChunkLoadingPattern();
            var checkerboard = new CheckerboardChunkLoadingPattern();
            var invertedLinear = new InvertedLinearChunkLoadingPattern();
            var linear = new LinearChunkLoadingPattern();
            var random = new RandomChunkLoadingPattern();
            var split = new SplitChunkLoadingPattern();
            AbstractRepository<IChunkLoadingPattern> ret = new(random);
            ret.Register(alternateCheckerboard.PatternName, alternateCheckerboard);
            ret.Register(checkerboard.PatternName, checkerboard);
            ret.Register(invertedLinear.PatternName, invertedLinear);
            ret.Register(linear.PatternName, linear);
            ret.Register(random.PatternName, random);
            ret.Register(split.PatternName, split);
            return ret;
        });
    }
}
