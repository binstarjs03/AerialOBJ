using Autofac;

using binstarjs03.AerialOBJ.WpfAppNew2.Components;
using binstarjs03.AerialOBJ.WpfAppNew2.ViewModels;
using binstarjs03.AerialOBJ.WpfAppNew2.Views;

namespace binstarjs03.AerialOBJ.WpfAppNew2;
public static class ContainerConfig
{
    public static IContainer Configure()
    {
        ContainerBuilder builder = new();

        builder.RegisterType<GlobalState>().AsSelf().SingleInstance();
        builder.RegisterType<MainWindow>().AsSelf();
        builder.RegisterType<MainViewModel>().AsSelf();

        return builder.Build();
    }
}
