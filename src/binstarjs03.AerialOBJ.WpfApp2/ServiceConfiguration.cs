using System;

using binstarjs03.AerialOBJ.MVVM;
using binstarjs03.AerialOBJ.MVVM.Services;
using binstarjs03.AerialOBJ.MVVM.Services.IOService;
using binstarjs03.AerialOBJ.MVVM.Services.ModalServices;
using binstarjs03.AerialOBJ.MVVM.ViewModels;
using binstarjs03.AerialOBJ.WpfApp.Services;
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
        services.AddSingleton<SharedViewModelState>();
    }

    private static void ConfigureViews(this IServiceCollection services)
    {
        services.AddTransient<AboutWindow>();
        services.AddSingleton<DebugLogWindow>();
    }

    private static void ConfigureViewModels(this IServiceCollection services)
    {
        services.AddTransient<ClosableViewModel>();
        services.AddSingleton<DebugLogViewModel>();
    }

    private static void ConfigureServices(this IServiceCollection services)
    {
        services.AddSingleton<ILogService, LogService>();
        services.AddSingleton<IModalService, ModalService>();
        services.AddSingleton<IAbstractIO, AbstractIO>();
    }
}
