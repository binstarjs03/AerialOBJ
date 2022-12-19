using System;
using System.Windows;

using binstarjs03.AerialOBJ.WpfAppNew2.Components;
using binstarjs03.AerialOBJ.WpfAppNew2.Factories;
using binstarjs03.AerialOBJ.WpfAppNew2.Services;
using binstarjs03.AerialOBJ.WpfAppNew2.ViewModels;
using binstarjs03.AerialOBJ.WpfAppNew2.Views;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Win32;

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
            services.ConfigureModalService();
            services.AddSingleton<ILogService, LogService>();
            services.AddSingleton<IIOService, IOService>();

        }).Build();
    }

    private static void ConfigureModalService(this IServiceCollection services)
    {
        services.AddSingleton<IModalService, ModalService>(x => new ModalService(
            GetAboutViewFactory(services),
            ShowMessageBoxHandler,
            ShowErrorMessageBoxHandler,
            ShowSaveFileDialogHandler
            ));

        static IAbstractFactory<IAboutView> GetAboutViewFactory(IServiceCollection services)
        {
            return services.BuildServiceProvider().GetRequiredService<IAbstractFactory<IAboutView>>();
        }

        static void ShowMessageBoxHandler(MessageBoxArg dialogArg)
        {
            MessageBox.Show(dialogArg.Message, dialogArg.Caption, MessageBoxButton.OK);
        }

        static void ShowErrorMessageBoxHandler(MessageBoxArg dialogArg)
        {
            MessageBox.Show(dialogArg.Message, dialogArg.Caption, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        static SaveFileDialogResult ShowSaveFileDialogHandler(SaveFileDialogArg dialogArg)
        {
            SaveFileDialog dialog = new()
            {
                FileName = dialogArg.FileName,
                DefaultExt = dialogArg.FileExtension,
                Filter = dialogArg.FileExtensionFilter
            };
            bool? result = dialog.ShowDialog();
            return new SaveFileDialogResult()
            {
                Path = dialog.FileName,
                Result = result == true,
            };
        }
    }
}
