using System;
using System.Windows;

using binstarjs03.AerialOBJ.WpfAppNew2.Components;
using binstarjs03.AerialOBJ.WpfAppNew2.Factories;
using binstarjs03.AerialOBJ.WpfAppNew2.Services;
using binstarjs03.AerialOBJ.WpfAppNew2.ViewModels;
using binstarjs03.AerialOBJ.WpfAppNew2.Views;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace binstarjs03.AerialOBJ.WpfAppNew2;
public static class AppHost
{
    public static IHost Configure()
    {
        return Host.CreateDefaultBuilder().ConfigureServices((hostContext, services) =>
        {
            // configure components
            services.AddSingleton<GlobalState>(x => new GlobalState(DateTime.Now));

            // configure models

            // configure views
            services.AddSingleton<MainView>();
            services.AddSingleton<DebugLogView>();
            services.AddAbstractFactory<IAboutView, AboutView>();

            // configure viewmodels
            services.AddSingleton<MainViewModel>();
            services.AddSingleton<DebugLogViewModel>();
            services.AddTransient<AboutViewModel>();

            // configure services
            services.AddSingleton<IModalService, ModalService>( x => new ModalService(
                services.BuildServiceProvider().GetRequiredService<IAbstractFactory<IAboutView>>(),
                msg => MessageBox.Show(msg)
            ));
            services.AddSingleton<ILogService, LogService>();

        }).Build();
    }
}
