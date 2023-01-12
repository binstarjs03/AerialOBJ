using System;

using binstarjs03.AerialOBJ.WpfApp.Factories;
using binstarjs03.AerialOBJ.WpfApp.Services.ChunkRendering;
using binstarjs03.AerialOBJ.WpfApp.Services.ModalServices;
using binstarjs03.AerialOBJ.WpfApp.Services.SavegameLoaderServices;
using binstarjs03.AerialOBJ.WpfApp.Services;
using binstarjs03.AerialOBJ.WpfApp.ViewModels;
using binstarjs03.AerialOBJ.WpfApp.Views;

using Microsoft.Extensions.DependencyInjection;
using System.IO;
using binstarjs03.AerialOBJ.WpfApp.Services.ChunkRegionProvider;
using binstarjs03.AerialOBJ.WpfApp.Services.Dispatcher;

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
            ISavegameLoaderService savegameLoaderService = x.GetRequiredService<ISavegameLoaderService>();
            IView viewportView = x.GetRequiredService<ViewportView>();
            return new MainViewModel(globalState, viewState, abstractViewModel, modalService, logService, savegameLoaderService, viewportView);
        });
        services.AddSingleton<DebugLogViewModel>();
        services.AddTransient<ViewportViewModel>();
        services.AddTransient<DefinitionManagerViewModel>();

        // configure services
        services.AddSingleton<IDispatcher, WpfDispatcher>(x=>new WpfDispatcher(App.Current.Dispatcher));
        services.AddSingleton<IModalService, ModalService>(x =>
        {
            IDialogView aboutViewFactory() => x.GetRequiredService<AboutView>();
            IDialogView definitionManagerViewFactory() => x.GetRequiredService<DefinitionManagerView>();
            return new ModalService(aboutViewFactory, definitionManagerViewFactory);
        });
        services.AddSingleton<IDefinitionManagerService, DefinitionManagerService>();
        services.AddSingleton<ILogService, LogService>();
        services.AddSingleton<IIOService, IOService>();
        services.AddSingleton<IRegionDiskLoader, RegionDiskLoader>();
        services.AddSingleton<ISavegameLoaderService, SavegameLoaderService>();
        services.AddTransient<IChunkRegionManagerService, ConcurrentChunkRegionManagerService>();
        services.AddSingleton<IChunkRenderer, ChunkRenderer>(x =>
        {
            // TODO We want to read on configuration file and choose which default chunk shader to instantiate.
            // For now default chunk shader is fixed to FlatChunkShader.
            IDefinitionManagerService definitionManager = x.GetRequiredService<IDefinitionManagerService>();
            IChunkShader shader = new FlatChunkShader(definitionManager);
            return new ChunkRenderer(shader);
        });
        services.AddSingleton<IViewportDefinitionLoaderService, ViewportDefinitionLoaderService>();

        return services.BuildServiceProvider();
    }
}