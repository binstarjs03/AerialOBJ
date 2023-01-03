using System;

using binstarjs03.AerialOBJ.WpfApp.Components;
using binstarjs03.AerialOBJ.WpfApp.Factories;
using binstarjs03.AerialOBJ.WpfApp.Services;
using binstarjs03.AerialOBJ.WpfApp.Services.ChunkRendering;
using binstarjs03.AerialOBJ.WpfApp.Services.ModalServices;
using binstarjs03.AerialOBJ.WpfApp.Services.SavegameLoaderServices;
using binstarjs03.AerialOBJ.WpfApp.ViewModels;
using binstarjs03.AerialOBJ.WpfApp.Views;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace binstarjs03.AerialOBJ.WpfApp;
public static class AppHost
{
    public static IHost Configure()
    {
        return Host.CreateDefaultBuilder().ConfigureServices((hostContext, services) =>
        {
            // configure components
            services.AddSingleton<GlobalState>(x => new GlobalState(DateTime.Now));
            services.AddSingleton<IMutableImageFactory, MutableImageFactory>();

            // configure models

            // configure factories
            services.AddSingleton<RegionModelFactory>();

            // configure views
            services.AddSingleton<MainView>();
            services.AddSingleton<DebugLogView>();
            services.AddTransient<AboutView>();
            services.AddTransient<DefinitionManagerView>();

            // configure viewmodels
            services.AddSingleton<MainViewModel>();
            services.AddSingleton<DebugLogViewModel>();
            services.AddTransient<AboutViewModel>();
            services.AddTransient<ViewportViewModel>();
            services.AddTransient<DefinitionManagerViewModel>();

            // configure services
            services.AddSingleton<IModalService, ModalService>(x =>
            {
                IDialogView aboutViewFactory() => x.GetRequiredService<AboutView>();
                IDialogView definitionManagerViewFactory() => x.GetRequiredService<DefinitionManagerView>();
                return new ModalService(aboutViewFactory, definitionManagerViewFactory);
            });
            services.AddSingleton<DefinitionManagerService>();
            services.AddSingleton<ILogService, LogService>();
            services.AddSingleton<IIOService, IOService>();
            services.AddSingleton<ISavegameLoaderService, SavegameLoaderService>();
            services.AddTransient<IChunkRegionManagerService, ConcurrentChunkRegionManagerService>();
            services.AddTransient<IChunkRegionManagerErrorMemory, ChunkRegionManagerErrorMemory>();
            services.AddSingleton<IRegionLoaderService, RegionLoaderService>();
            services.AddSingleton<IChunkRenderService, ChunkRenderService>(x =>
            {
                ServiceProvider provider = services.BuildServiceProvider();
                IChunkShader shader = new FlatChunkShader(provider.GetRequiredService<DefinitionManagerService>());
                return new ChunkRenderService(shader);
            });
        }).Build();
    }
}
