using System;

using Autofac;

using binstarjs03.AerialOBJ.WpfAppNew2.Components;
using binstarjs03.AerialOBJ.WpfAppNew2.Services;
using binstarjs03.AerialOBJ.WpfAppNew2.ViewModels;
using binstarjs03.AerialOBJ.WpfAppNew2.Views;

namespace binstarjs03.AerialOBJ.WpfAppNew2;
public static class ContainerConfig
{
    public static IContainer Configure()
    {
        ContainerBuilder builder = new();

        // register components
        builder.RegisterType<GlobalState>().AsSelf()
                                           .SingleInstance()
                                           .WithParameter("launchTime", DateTime.Now);

        // register services
        builder.RegisterType<ModalService>().As<IModalService>().SingleInstance();
        builder.RegisterType<LogService>().As<ILogService>().SingleInstance();

        // register MVVMs
        builder.RegisterType<MainViewModel>().AsSelf();
        builder.RegisterType<MainView>().AsSelf();
        builder.RegisterType<AboutView>().AsSelf();

        return builder.Build();
    }
}
