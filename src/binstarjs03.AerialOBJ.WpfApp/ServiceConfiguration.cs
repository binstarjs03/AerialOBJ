using System;

using binstarjs03.AerialOBJ.Imaging.ChunkRendering;
using binstarjs03.AerialOBJ.WpfApp.Factories;
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
    internal static IServiceProvider Configure(GlobalState globalState)
    {
        IServiceCollection services = new ServiceCollection();

        // configure application-wide state
        services.AddSingleton<GlobalState>(globalState);
        services.AddSingleton<SettingState>(globalState.Setting);
        services.AddSingleton<ViewState>();

        services.ConfigureFactories();
        services.ConfigureViews();
        services.ConfigureViewModels();
        services.ConfigureServices(globalState);

        return services.BuildServiceProvider();
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
    }

    internal static void ConfigureViewModels(this IServiceCollection services)
    {
        services.AddSingleton<AbstractViewModel>();
        services.AddSingleton<MainViewModel>(x =>
        {
            GlobalState globalState = x.GetRequiredService<GlobalState>();
            ViewState viewState = x.GetRequiredService<ViewState>();
            AbstractViewModel abstractViewModel = x.GetRequiredService<AbstractViewModel>();
            IModalService modalService = x.GetRequiredService<IModalService>();
            ILogService logService = x.GetRequiredService<ILogService>();
            ISavegameLoader savegameLoaderService = x.GetRequiredService<ISavegameLoader>();
            IView viewportView = x.GetRequiredService<ViewportView>();
            return new MainViewModel(globalState, viewState, abstractViewModel, modalService, logService, savegameLoaderService, viewportView);
        });
        services.AddSingleton<DebugLogViewModel>();

        services.AddTransient<ViewportViewModel>();
        services.AddTransient<ViewportViewModelInputHandler>();
        services.AddTransient<DefinitionManagerViewModel>();
    }

    internal static void ConfigureServices(this IServiceCollection services, GlobalState globalState)
    {
        services.AddSingleton<IDispatcher, WpfDispatcher>(x => new WpfDispatcher(App.Current.Dispatcher));
        services.AddSingleton<IModalService, ModalService>(x =>
        {
            IDialogView aboutViewFactory() => x.GetRequiredService<AboutView>();
            IDialogView definitionManagerViewFactory() => x.GetRequiredService<NewDefinitionManagerView>();
            return new ModalService(aboutViewFactory, definitionManagerViewFactory);
        });
        services.AddSingleton<IDefinitionManager, DefinitionManager>();
        services.AddSingleton<ILogService, LogService>();
        services.AddSingleton<IAbstractIO, AbstractIO>();
        services.AddSingleton<IRegionDiskLoader, RegionDiskLoader>();
        services.AddSingleton<ISavegameLoader, SavegameLoader>();
        services.AddSingleton<IDefinitionIO, DefinitionIO>(x => new DefinitionIO(globalState.Path.DefinitionsPath));

        services.AddTransient<IChunkRegionManager, ChunkRegionManager>();
        services.AddTransient<IChunkLoadingPattern, RandomChunkLoadingPattern>();
        services.AddSingleton<IChunkRenderer, ChunkRenderer>(x =>
        {
            SettingState setting = x.GetRequiredService<SettingState>();
            IChunkShader shader = new StandardChunkShader();
            return new ChunkRenderer(shader, setting);
        });
        services.AddTransient<IKeyHandler, KeyHandler>();
        services.AddTransient<IMouseHandler, MouseHandler>();
    }
}
