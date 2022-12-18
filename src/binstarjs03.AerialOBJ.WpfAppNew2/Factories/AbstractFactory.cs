using System;

using Autofac;

namespace binstarjs03.AerialOBJ.WpfAppNew2.Factories;
public class AbstractFactory<T> : IAbstractFactory<T>
{
    private readonly Func<T> _factory;

    public AbstractFactory(Func<T> factory)
    {
        _factory = factory;
    }

    public T Create()
    {
        return _factory();
    }
}

public static class AbstractFactoryExtension
{
    public static void RegisterAbstractFactory<TInterface, TImplementation>(this ContainerBuilder builder)
        where TInterface : class
        where TImplementation : class, TInterface
    {
        builder.RegisterType<TImplementation>().As<TInterface>().InstancePerDependency();
        builder.Register<Func<TInterface>>(c => () => c.Resolve<TInterface>())
               .As<Func<TInterface>>()
               .SingleInstance();
        //builder.RegisterType<AbstractFactory<TInterface>>().As<IAbstractFactory<TInterface>>().SingleInstance();
    }
}