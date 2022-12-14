using System;

using binstarjs03.AerialOBJ.WpfApp.Components;
using binstarjs03.AerialOBJ.WpfApp.Factories;
using binstarjs03.AerialOBJ.WpfApp.Services.ChunkRendering;
using binstarjs03.AerialOBJ.WpfApp.Services.ModalServices;
using binstarjs03.AerialOBJ.WpfApp.Services.SavegameLoaderServices;
using binstarjs03.AerialOBJ.WpfApp.Services;
using binstarjs03.AerialOBJ.WpfApp.ViewModels;
using binstarjs03.AerialOBJ.WpfApp.Views;

using Microsoft.Extensions.DependencyInjection;
using System.IO;

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
        services.AddTransient<IChunkRegionManagerErrorMemory, ChunkRegionManagerErrorMemory>();

        // configure models
        // models configuration goes here

        // configure factories
        services.AddSingleton<RegionModelFactory>();
        services.AddSingleton<IRegionImageFactory, RegionImageFactory>();

        // configure views
        services.AddSingleton<MainView>();
        services.AddSingleton<DebugLogView>();
        services.AddTransient<AboutView>();
        services.AddTransient<DefinitionManagerView>();
        services.AddTransient<ViewportView>();

        // configure viewmodels
        services.AddSingleton<AbstractViewModel>();
        services.AddSingleton<MainViewModel>(x =>
        {
            GlobalState globalState = x.GetRequiredService<GlobalState>();
            ViewState viewState = x.GetRequiredService<ViewState>();
            IModalService modalService = x.GetRequiredService<IModalService>();
            ILogService logService = x.GetRequiredService<ILogService>();
            ISavegameLoaderService savegameLoaderService = x.GetRequiredService<ISavegameLoaderService>();
            IView viewportView = x.GetRequiredService<ViewportView>();
            return new MainViewModel(globalState, viewState, modalService, logService, savegameLoaderService, viewportView);
        });
        services.AddSingleton<DebugLogViewModel>();
        services.AddTransient<ViewportViewModel>();
        services.AddTransient<DefinitionManagerViewModel>();

        // configure services
        services.AddSingleton<IModalService, ModalService>(x =>
        {
            IDialogView aboutViewFactory() => x.GetRequiredService<AboutView>();
            IDialogView definitionManagerViewFactory() => x.GetRequiredService<DefinitionManagerView>();
            return new ModalService(aboutViewFactory, definitionManagerViewFactory);
        });
        services.AddSingleton<IDefinitionManagerService, DefinitionManagerService>();
        services.AddSingleton<ILogService, LogService>();
        services.AddSingleton<IIOService, IOService>();
        services.AddSingleton<ISavegameLoaderService, SavegameLoaderService>();
        services.AddTransient<IChunkRegionManagerService, ConcurrentChunkRegionManagerService>();
        services.AddSingleton<IRegionLoaderService, RegionLoaderService>();
        services.AddSingleton<IChunkRenderService, ChunkRenderService>(x =>
        {
            // TODO We want to read on configuration file and choose which default chunk shader to instantiate.
            // For now default chunk shader is fixed to FlatChunkShader.
            IDefinitionManagerService definitionManager = x.GetRequiredService<IDefinitionManagerService>();
            IChunkShader shader = new FlatChunkShader(definitionManager);
            return new ChunkRenderService(shader);
        });
        services.AddSingleton<IViewportDefinitionLoaderService, ViewportDefinitionLoaderService>();

        return services.BuildServiceProvider();
    }
}