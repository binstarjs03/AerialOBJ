using System;
using System.IO;

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
    internal static IServiceProvider Configure()
    {
        IServiceCollection services = new ServiceCollection();

        // configure components
        services.AddSingleton<GlobalState>(x =>
        {
            string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string definitionDirectoryPath = Path.Combine(currentDirectory, "Definitions");
            return new GlobalState(DateTime.Now, "Alpha", currentDirectory, definitionDirectoryPath);
        });
        services.AddSingleton<ViewState>();

        // configure models
        // models configuration goes here

        // configure factories
        services.AddSingleton<RegionDataImageModelFactory>();
        services.AddSingleton<IRegionImageFactory, RegionImageFactory>();

        // configure views
        services.AddSingleton<MainView>();
        services.AddSingleton<DebugLogView>();
        services.AddTransient<AboutView>();
        services.AddTransient<DefinitionManagerView>();
        services.AddTransient<ViewportView>();

        // configure viewmodels
        services.AddSingleton<AbstractViewModel>();
        services.AddSingleton<IMainViewModel, MainViewModel>(x =>
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

        // configure services
        services.AddSingleton<IDispatcher, WpfDispatcher>(x => new WpfDispatcher(App.Current.Dispatcher));
        services.AddSingleton<IModalService, ModalService>(x =>
        {
            IDialogView aboutViewFactory() => x.GetRequiredService<AboutView>();
            IDialogView definitionManagerViewFactory() => x.GetRequiredService<DefinitionManagerView>();
            return new ModalService(aboutViewFactory, definitionManagerViewFactory);
        });
        services.AddSingleton<IDefinitionManager, DefinitionManager>();
        services.AddSingleton<ILogService, LogService>();
        services.AddSingleton<IAbstractIO, AbstractIO>();
        services.AddSingleton<IRegionDiskLoader, RegionDiskLoader>();
        services.AddSingleton<ISavegameLoader, SavegameLoader>();
        services.AddTransient<IChunkRegionManager, ChunkRegionManager>();
        services.AddTransient<IChunkLoadingPattern, RandomChunkLoadingPattern>();
        services.AddSingleton<IChunkRenderer, ChunkRenderer>(x =>
        {
            // TODO We want to read on configuration file and choose which default chunk shader to instantiate.
            // For now default chunk shader is fixed.
            IDefinitionManager definitionManager = x.GetRequiredService<IDefinitionManager>();
            IChunkShader shader = new StandardChunkShader();
            return new ChunkRenderer(shader, definitionManager);
        });
        services.AddSingleton<IDefinitionIO, DefinitionIO>();
        services.AddTransient<IKeyHandler, KeyHandler>();
        services.AddTransient<IMouseHandler, MouseHandler>();

        return services.BuildServiceProvider();
    }
}
